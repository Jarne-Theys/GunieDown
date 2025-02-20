using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AIPlayerAgent : Agent
{
    public Transform goal;
    public Rigidbody rb;
    public float moveSpeed = 3f;
    public float strafeSpeed = 2f; // Separate strafe speed if needed
    public float rotationSpeed = 200f;

    private Vector3 lastPosition;

    public override void OnEpisodeBegin()
    {
        transform.position = SpawnPositions.aiPlayerSpawn; // Make sure SpawnPositions is correctly defined or replace with Vector3.zero for testing
        transform.rotation = Quaternion.identity;
        lastPosition = transform.position;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);  // AI position
        sensor.AddObservation(goal.position);      // Goal position
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveForwardBackward = actions.ContinuousActions[0]; // Forward/Backward movement
        float moveLeftRight = actions.ContinuousActions[1];     // Strafing movement

        // **Always Look at Goal:**
        transform.LookAt(goal);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f); // Lock rotation to Y axis only if needed

        // Movement based on actions (now strafing is possible)
        Vector3 moveDirectionForward = transform.forward * moveForwardBackward * moveSpeed;
        Vector3 moveDirectionStrafe = transform.right * moveLeftRight * strafeSpeed;

        Vector3 finalMoveDirection = moveDirectionForward + moveDirectionStrafe;

        float gravity = rb.linearVelocity.y; // Preserve gravity
        rb.linearVelocity = new Vector3(finalMoveDirection.x, gravity, finalMoveDirection.z);


        // Reward for getting closer to goal (same as before)
        float previousDistance = Vector3.Distance(lastPosition, goal.position);
        float currentDistance = Vector3.Distance(transform.position, goal.position);

        if (currentDistance < previousDistance)
        {
            SetReward(0.01f);
        }
        else
        {
            SetReward(-0.01f);
        }

        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            SetReward(+1f);
            EndEpisode();
        }

        if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical");   // Forward/Backward using W/S or Up/Down
        continuousActions[1] = Input.GetAxis("Horizontal"); // Strafe using A/D or Left/Right
    }
}