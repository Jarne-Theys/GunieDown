using UnityEngine;

public class DashBackwardComponent : IUpgradeComponent
{
    public float DashForce { get; private set; }
    
    public DashBackwardComponent(float dashForce)
    {
        DashForce = dashForce;
    }
    
    public void ApplyPassive(GameObject player) { }
    
    public void Activate(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(-player.transform.forward * DashForce, ForceMode.Impulse);
        }
    }
}