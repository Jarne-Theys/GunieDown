using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UpwardForceUpgrade", menuName = "Scripts/Upgrades/Upward Force")]
public class UpwardForceUpgradeDefinition : UpgradeDefinition
{
    private void OnEnable()
    {
        if (components.Count == 0)
        {
            UpwardForceComponent upwardForceComponent = new UpwardForceComponent(8f);
            
            InputActivationComponent inputActivationComponent = new InputActivationComponent(upwardForceComponent.Activate);
            components.Add(inputActivationComponent);
        }
    }
}