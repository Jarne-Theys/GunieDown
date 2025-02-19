using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

public class AIPlayerAgent : Agent
{
    public Transform enemy;
    public float moveSpeed = 3f;
    public float rotationSpeed = 200f;

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);  // AI position
        sensor.AddObservation(enemy.position);  // Enemy position
        sensor.AddObservation(Vector3.Distance(transform.position, enemy.position)); // Distance to enemy
        sensor.AddObservation(transform.forward);  // Direction AI is facing

        Debug.Log("Observations collected");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];

        Debug.Log($"Move: {move}, Rotate: {rotate}");

        transform.Rotate(0, rotate * rotationSpeed * Time.deltaTime, 0);
        transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical");
        continuousActions[1] = Input.GetAxis("Horizontal");
    }
}

/*
public class AIPlayerAgent : Agent
{
    public Transform enemy;

    private Rigidbody rb;

    public float moveForce = 5f;
    public float rotationSpeed = 100f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(enemy.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];

        rb.angularVelocity = new Vector3(0, rotate * rotationSpeed * Time.deltaTime, 0);

        Vector3 movement = transform.forward * move * moveForce;
        rb.AddForce(movement, ForceMode.Acceleration);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical");
        continuousActions[1] = Input.GetAxis("Horizontal");
    }
}
*/