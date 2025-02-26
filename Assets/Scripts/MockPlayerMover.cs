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
        float moveX = Random.Range(-moveRange, moveRange) * Time.deltaTime * moveSpeed;
        float moveY = 0f;
        float moveZ = Random.Range(-moveRange, moveRange) * Time.deltaTime * moveSpeed;

        Vector3 movement = new Vector3(moveX, moveY, moveZ);
        rb.MovePosition(transform.position + movement);
    }
}
