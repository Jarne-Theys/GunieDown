using UnityEngine;

public class UpwardForceComponent : UpgradeComponentBase
{
    [SerializeField]
    private float moveForce;
    
    private bool inputInitialized = false;

    public UpwardForceComponent(float moveForce)
    {
        this.moveForce = moveForce;
    }
    
    public override void ApplyPassive(GameObject player)
    {
    }

    public override void Activate(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * moveForce, ForceMode.Impulse);
        }
    }
}
