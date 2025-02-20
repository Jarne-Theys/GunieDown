using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AIPlayerAgent : Agent
{
    public Transform goal;
    public Rigidbody rb;
    public float moveSpeed = 3f;

    public float raycastDistance = 5f; // Adjust as needed
    public int numRaycasts = 3; // Number of rays to cast (e.g., forward, left, right)

    private Vector3 lastPosition;

    private float timer = 0f;

    public override void OnEpisodeBegin()
    {
        transform.position = SpawnPositions.aiPlayerSpawnPosition;
        transform.rotation = Quaternion.Euler(SpawnPositions.aiPlayerSpawnRotation);
        lastPosition = transform.position;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(goal.position);

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
                if (hit.collider.CompareTag("Wall")) // Make sure your walls have the "Wall" tag
                {
                    wallDetected = true;
                    distanceToWall = hit.distance;
                }
            }

            sensor.AddObservation(wallDetected ? 1f : 0f); // Binary: 1 if wall detected, 0 otherwise
            sensor.AddObservation(distanceToWall / raycastDistance); // Normalized distance to wall (0 to 1)
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveForwardBackward = actions.ContinuousActions[0]; // Forward/Backward movement
        float moveLeftRight = actions.ContinuousActions[1];     // Strafing movement

        Vector3 moveDirectionForward = transform.forward * moveForwardBackward * moveSpeed;
        Vector3 moveDirectionStrafe = transform.right * moveLeftRight * moveSpeed;

        Vector3 finalMoveDirection = moveDirectionForward + moveDirectionStrafe;

        float gravity = rb.linearVelocity.y; // Preserve gravity
        rb.linearVelocity = new Vector3(finalMoveDirection.x, gravity, finalMoveDirection.z);


        float previousDistance = Vector3.Distance(lastPosition, goal.position);
        float currentDistance = Vector3.Distance(transform.position, goal.position);

        if (currentDistance < previousDistance)
        {
            AddReward(0.1f);
        }
        else
        {
            AddReward(-0.1f);
        }

        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            timer = 0f;
            AddReward(-0.0001f);
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
}