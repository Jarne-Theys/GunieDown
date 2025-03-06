using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

[Serializable]
public class DashBackwardComponent : UpgradeComponentBase
{
    [SerializeField]
    private float dashForce;

    public DashBackwardComponent() {}

    public override void ApplyPassive(GameObject player) {}

    public override void Activate(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(-player.transform.forward * dashForce, ForceMode.Impulse);
            return;
        }

        rb = player.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(-player.transform.forward * dashForce, ForceMode.Impulse);
            return;
        }
    }
}