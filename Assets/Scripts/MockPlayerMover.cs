using UnityEngine;

public class MockPlayerMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveRange = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        RandomMove();
    }

    private void RandomMove()
    {
        float moveX = Random.Range(-moveRange, moveRange) / moveSpeed;
        float moveZ = Random.Range(-moveRange, moveRange) / moveSpeed;


        rb.AddForce(new Vector3(moveX, 0, moveZ), ForceMode.Impulse);
    }
}
