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
    public int numWallRaycasts;

    public GameObject target;

    private float totalTime = 0f;
    private int episodeDuration = 60;

    public int bulletTrackCount;
    public float fovAngle;
    public float visionRange;
    private Vector3 lastKnownPlayerLocation = SpawnPositions.mockHumanPlayerSpawn;

    public float fireRate = 1f;
    private float fireCooldown = 0f;

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
        fireCooldown = 0f;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Add a reward from external scripts
    // currently only used by bullets (when they hit the agent)
    public void AddExternalReward(float reward)
    {
        AddReward(reward);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Transform Observations
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.forward); // 3 floats
        sensor.AddObservation(transform.up);    // 3 floats (optional if always upright)

        // Raycast Observations
        float angleStep = 360f / numWallRaycasts;
        for (int i = 0; i < numWallRaycasts; i++)
        {
            float currentAngle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;
            // Simplified direction calculation:
            // Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            Vector3 rayFrom = transform.position; // Or your offset: transform.position - Vector3.up * 0.5f; Clarify why the offset?
            RaycastHit hit;
            bool wallDetected = false;
            float distanceToHit = raycastDistance; // Default to max distance

            if (Physics.Raycast(rayFrom, rayDirection, out hit, raycastDistance))
            {
                // Consider layers or specific tags if more than just "Wall" exists
                if (hit.collider.CompareTag("Wall"))
                {
                    wallDetected = true;
                    distanceToHit = hit.distance;
                }
                // Optional: Observe other object types? (e.g., obstacles, cover)
                // sensor.AddObservation(hit.collider.CompareTag("Obstacle")); // Example
            }
            sensor.AddObservation(wallDetected); // Bool observation
            sensor.AddObservation(distanceToHit / raycastDistance); // Normalized distance
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

        var directionToPlayerNormalized = (lastKnownPlayerLocation - transform.position).normalized;
        sensor.AddObservation(lastKnownPlayerLocation);
        sensor.AddObservation(directionToPlayerNormalized);

        sensor.AddObservation(fireCooldown < 0f ? true : false);

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

            var angleReward = 1 - (angleToPlayer / (fovAngle / 2));
            AddReward(angleReward/10);

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
        } else
        {
            AddReward(-0.01f);
        }

        bool fire = actions.DiscreteActions[0] == 1;

        if (fire && fireCooldown <= 0f)
        {
            if (playerInFOV)
            {
                Ray ray = new Ray(transform.position, directionToPlayer);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, visionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        AddReward(1f);
                        fireCooldown = 1f;
                    }
                }
            }
        }
        else
        {
        }

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(finalMoveDirection.x, rb.linearVelocity.y, finalMoveDirection.z);

        totalTime += Time.fixedDeltaTime;
        if (totalTime > episodeDuration)
        {
            EndEpisode();
        }

        fireCooldown -= Time.fixedDeltaTime;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-10f);
            Debug.DrawLine(collision.transform.position, collision.transform.position + (Vector3.up * 20f), Color.black);
            return;
        }
    }
}