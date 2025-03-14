using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Threading;
using UnityEditor;

public class AIPlayerAgent : Agent
{
    public Rigidbody rb;
    private Vector3 finalMoveDirection;
    public float moveSpeed;

    public float raycastDistance;
    public int numRaycasts;

    public GameObject target;

    // private Vector3 lastPosition;

    //private float timer = 0f;
    private float totalTime = 0f;
    private int episodeDuration = 60;

    public int bulletTrackCount;
    public float fovAngle;
    public float visionRange;
    private Vector3 lastKnownPlayerLocation = Vector3.zero;

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = SpawnPositions.aiPlayerSpawnPosition;
        transform.rotation = Quaternion.Euler(SpawnPositions.aiPlayerSpawnRotation);

        target.transform.position = SpawnPositions.mockHumanPlayerSpawn;
        target.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        BulletTracker.ClearTrackedBulletList();

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        totalTime = 0f;
        // timer = 0f;
        // lastPosition = transform.position;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AddExternalReward(float reward)
    {
        AddReward(reward);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Transform Observations
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);

        // Raycast Observations
        for (float currentAngle = 0; currentAngle <= 360; currentAngle += 90)
        {
            float angle = currentAngle;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;
            Vector3 rayFrom = transform.position - Vector3.up * 0.5f;
            Ray ray = new Ray(rayFrom, rayDirection);
            RaycastHit hit;

            bool wallDetected = false;
            float distanceToWall = raycastDistance;

            if (Physics.Raycast(rayFrom, rayDirection, out hit, raycastDistance))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    wallDetected = true;
                    distanceToWall = hit.distance;
                }
            }

            sensor.AddObservation(wallDetected ? true : false);
            sensor.AddObservation(distanceToWall / raycastDistance); // Normalized distance to wall (0 to 1)
        }

        // Bullet tracking
        for (int i = 0; i < bulletTrackCount; i++)
        {
            if (i < BulletTracker.trackedBullets.Count)
            {
                sensor.AddObservation(BulletTracker.trackedBullets[i].position - transform.position);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
            }
        }

        // Target Observations
        Vector3 playerCenter = target.GetComponentInChildren<CapsuleCollider>().bounds.center;
        Vector3 directionToPlayer = (playerCenter - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        bool playerInFOV = angleToPlayer < fovAngle / 2 &&
                           Vector3.Distance(transform.position, target.transform.position) <= visionRange;

        bool playerVisible = false;

        if (playerInFOV)
        {
            Ray ray = new Ray(transform.position, directionToPlayer);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange))
            {
                if (hit.collider.CompareTag("Player")) 
                {
                    lastKnownPlayerLocation = hit.point;
                    playerVisible = true;
                }
            }
        }

        sensor.AddObservation(playerVisible);
        sensor.AddObservation(lastKnownPlayerLocation);
        /**
         * @todo Identify extra 2 observations being added by Unity. 
         * @body Check why unity is saying 30 observations made, while this code only makes 28, as shown by this debug log. Currently just set to 30 in editor, might cause issues later.
         */
        // Debug.Log($"Total Observations: {sensor.ObservationSize()}");

    }

    void DrawWallDetectionLinesV2()
    {
        for (float currentAngle = 0; currentAngle <=360; currentAngle+=90)
        {
            float angle = currentAngle;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;
            Vector3 rayFrom = transform.position - Vector3.up * 0.5f;
            Ray ray = new Ray(rayFrom, rayDirection);
            Gizmos.DrawRay(ray.origin, ray.direction * raycastDistance);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        DrawWallDetectionLinesV2();

        if (target == null) return;

        Vector3 playerCenter = target.GetComponentInChildren<CapsuleCollider>().bounds.center;
        Vector3 directionToPlayer = (playerCenter - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        bool playerInFOV = angleToPlayer < fovAngle / 2 &&
                           Vector3.Distance(transform.position, target.transform.position) <= visionRange;

        if (playerInFOV)
        {
            Ray ray = new Ray(transform.position, directionToPlayer);
            //Debug.DrawRay(transform.position, directionToPlayer * visionRange, Color.blue);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange))
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, hit.point);
                if (hit.collider.CompareTag("Player"))
                {
                    Gizmos.color = Color.green;
                    //Gizmos.DrawLine(transform.position, playerCenter);
                } 
                else
                {
                    Gizmos.color = Color.yellow;
                }
            }
        } 
        else
        {
            Gizmos.color = Color.red;
        }

        DrawVisionCone();

    }



    private void DrawVisionCone()
    {
        int segments = 10; // How smooth the cone should be
        float stepAngle = fovAngle / segments;
        Vector3 startDirection = Quaternion.Euler(0, -fovAngle / 2, 0) * transform.forward;

        Vector3 previousPoint = transform.position + startDirection * visionRange;
        Gizmos.DrawLine(previousPoint, transform.position);
        for (int i = 1; i <= segments; i++)
        {
            float angle = -fovAngle / 2 + stepAngle * i;
            Vector3 nextDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 nextPoint = transform.position + nextDirection * visionRange;

            Gizmos.DrawLine(transform.position, nextPoint);
            Gizmos.DrawLine(previousPoint, nextPoint);

            previousPoint = nextPoint;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Rotation
        float YRotationInput = actions.ContinuousActions[2]; // Camera rotation around y-axis
        float YRotation = YRotationInput * 180f; // Convert from -1 to 1 to -180 to 180
        transform.localEulerAngles = new Vector3(0f, YRotation, 0f);

        // Movement
        float moveForwardBackward = actions.ContinuousActions[0]; // Forward/Backward movement
        float moveLeftRight = actions.ContinuousActions[1];     // Strafing movement

        Vector3 moveDirectionForward = transform.forward * moveForwardBackward * moveSpeed;
        Vector3 moveDirectionStrafe = transform.right * moveLeftRight * moveSpeed;

        finalMoveDirection = moveDirectionForward + moveDirectionStrafe;

        Vector3 playerCenter = target.GetComponentInChildren<CapsuleCollider>().bounds.center;
        Vector3 directionToPlayer = (playerCenter - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        bool playerInFOV = angleToPlayer < fovAngle / 2 &&
                           Vector3.Distance(transform.position, target.transform.position) <= visionRange;

        if (playerInFOV)
        {
            AddReward(0.01f);
            /*
            Ray ray = new Ray(transform.position, directionToPlayer);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green, 15f);
                    AddReward(0.01f);
                }
                else
                {
                    
                }
            }
            */
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(finalMoveDirection.x, rb.linearVelocity.y, finalMoveDirection.z);

        totalTime += Time.deltaTime;
        if (totalTime > episodeDuration)
        {
            EndEpisode();
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-10f);
            Debug.DrawLine(collision.transform.position, collision.transform.position + (Vector3.up * 20f), Color.black);
            return;
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            AddReward(-10);
            Debug.DrawLine(collision.transform.position, collision.transform.position + (Vector3.up * 20f), Color.blue);
            return;
        }
    }
}