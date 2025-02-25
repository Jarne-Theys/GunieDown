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
    public float moveSpeed = 3f;

    public float raycastDistance = 5f;
    public int numRaycasts = 3;

    public GameObject target;

    // private Vector3 lastPosition;

    //private float timer = 0f;
    private float totalTime = 0f;
    private int episodeDuration = 60;

    public int bulletTrackCount = 3;
    public float fovAngle = 60f;
    public float visionRange = 50f;

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = SpawnPositions.aiPlayerSpawnPosition;
        transform.rotation = Quaternion.Euler(SpawnPositions.aiPlayerSpawnRotation);

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
        for (int i = 0; i < numRaycasts; i++)
        {
            float angle = (i - (numRaycasts - 1) / 2f) * 30f; // Example angles: -30, 0, 30 degrees
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;
            RaycastHit hit;
            bool wallDetected = false;
            float distanceToWall = raycastDistance;

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, rayDirection, out hit, raycastDistance)) // Offset raycast start slightly up to avoid ground issues
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    wallDetected = true;
                    distanceToWall = hit.distance;
                }
            }

            sensor.AddObservation(wallDetected ? 1f : 0f); // Binary: 1 if wall detected, 0 otherwise
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
        Vector3 directionToPlayer = (target.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < numRaycasts; i++)
        {
            float angle = (i - (numRaycasts - 1) / 2f) * 30f; // Example angles: -30, 0, 30 degrees
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;

            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f + rayDirection * raycastDistance);
        }

        // Ensure target exists
        if (target == null) return;

        Vector3 playerCenter = target.GetComponentInChildren<CapsuleCollider>().bounds.center; // Use collider center instead of transform.position
        Vector3 directionToPlayer = (playerCenter - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check if player is within the FOV angle
        bool playerInFOV = angleToPlayer < fovAngle / 2 &&
                           Vector3.Distance(transform.position, target.transform.position) <= visionRange;

        if (playerInFOV)
        {
            // Perform a raycast from AI to the player's collider center
            Ray ray = new Ray(transform.position, directionToPlayer);
            //Debug.DrawRay(transform.position, directionToPlayer * visionRange, Color.blue); // Debugging ray

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange))
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, hit.point);
                if (hit.collider.CompareTag("Player")) // Ensure the first hit object is the player
                {

                    // Player is in direct sight
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

        // Draw the vision cone
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


        /*
        if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
        {
            AddReward(-0.0001f);
        }

        lastPosition = transform.position;
        */
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(finalMoveDirection.x, rb.linearVelocity.y, finalMoveDirection.z);
        /*
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            timer = 0f;
            AddReward(0.0001f);
        }
        */

        totalTime += Time.deltaTime;
        if (totalTime > episodeDuration)
        {
            EndEpisode();
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        AddReward(-10f);
    }
    

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TrainZone"))
        {
            AddReward(-10f);
            EndEpisode();
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            AddReward(+10f);
            EndEpisode();
        }

        if (other.CompareTag("Wall"))
        {
            SetReward(-10f);
            //EndEpisode();
        }
    }
    */
}