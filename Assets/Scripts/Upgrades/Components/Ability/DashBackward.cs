using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;

[Serializable]
public class DashBackward : UpgradeComponentBase
{
    [SerializeField]
    protected float dashForce;

    public DashBackward() {}

    protected override void ExecuteActivation(GameObject player, List<IUpgradeComponent> runtimeComponents)
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