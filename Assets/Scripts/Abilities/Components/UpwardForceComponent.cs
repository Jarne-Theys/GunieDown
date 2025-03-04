using UnityEngine;

public class UpwardForceComponent : IAbilityComponent
{
    public float ForceAmount { get; private set; }

    public UpwardForceComponent(float forceAmount)
    {
        ForceAmount = forceAmount;
    }

    public void Activate(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * ForceAmount, ForceMode.Impulse);
    }
}
