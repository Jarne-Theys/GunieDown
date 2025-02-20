using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AIPlayerAgent : Agent
{
    public Transform goal;
    public Rigidbody rb;
    public float moveSpeed = 3f;
    public float rotationSpeed = 200f;

    public override void OnEpisodeBegin()
    {
        transform.position = SpawnPositions.aiPlayerSpawn;
        transform.rotation = Quaternion.identity;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);  // AI position
        sensor.AddObservation(goal.position);      // Goal position

        //Debug.Log("Observations collected");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];  // Left/right movement
        float moveZ = actions.ContinuousActions[1];  // Forward movement
        float rotateY = actions.ContinuousActions[2]; // Rotation

        // Convert input into velocity
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 localMove = transform.TransformDirection(moveDirection) * moveSpeed;

        // Apply gravity manually
        float gravity = Physics.gravity.y;  // Unity's gravity value (-9.81 by default)
        rb.linearVelocity = new Vector3(localMove.x, gravity, localMove.z);

        // Rotate based on input
        transform.Rotate(0, rotateY * rotationSpeed * Time.deltaTime, 0);
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

        if (goal == null)
        {
            continuousActions[0] = 0f;
            continuousActions[1] = 0f;
            continuousActions[2] = 0f;
            return;
        }

        // Compute direction to goal
        Vector3 directionToGoal = (goal.position - transform.position).normalized;

        // Convert direction to local space
        Vector3 localDirection = transform.InverseTransformDirection(directionToGoal);

        // Move towards the goal (strafe left/right and move forward)
        continuousActions[0] = localDirection.x; // Strafe
        continuousActions[1] = localDirection.z; // Move forward

        // Rotate toward the goal
        float targetAngle = Mathf.Atan2(directionToGoal.x, directionToGoal.z) * Mathf.Rad2Deg;
        float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        continuousActions[2] = angleDifference / 180f; // Normalize to [-1,1]
    }
}
