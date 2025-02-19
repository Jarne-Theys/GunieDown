using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

public class AIPlayerAgent : Agent
{
    public Transform enemy;
    public Rigidbody rb;
    public float moveSpeed = 3f;
    public float rotationSpeed = 200f;

    public override void OnEpisodeBegin()
    {
        transform.position = SpawnPositions.aiPlayerSpawn;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);  // AI position
        sensor.AddObservation(enemy.position);  // Enemy position
        //sensor.AddObservation(Vector3.Distance(transform.position, enemy.position)); // Distance to enemy
        //sensor.AddObservation(transform.forward);  // Direction AI is facing

        Debug.Log("Observations collected");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float rotateY = actions.ContinuousActions[2];

        rb.linearVelocity = new Vector3(moveX * moveSpeed, rb.linearVelocity.y, moveZ * moveSpeed);

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
        continuousActions[0] = Input.GetAxis("Vertical");
        continuousActions[1] = Input.GetAxis("Horizontal");
    }
    
}