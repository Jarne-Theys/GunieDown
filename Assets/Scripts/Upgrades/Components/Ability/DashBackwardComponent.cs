using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

[Serializable]
public class DashBackwardComponent : UpgradeComponentBase
{
    [SerializeField]
    private float dashForce;
    
    private bool inputInitialized = false;

    public DashBackwardComponent(float dashForce)
    {
        this.dashForce = dashForce;
    }

    public override void ApplyPassive(GameObject player)
    {
    }

    public override void Activate(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(-player.transform.forward * dashForce, ForceMode.Impulse);
        }
    }
}