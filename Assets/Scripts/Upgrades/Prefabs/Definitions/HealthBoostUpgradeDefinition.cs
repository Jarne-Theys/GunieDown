using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "HealthBoostUpgrade", menuName = "Scripts/Upgrades/Health Boost Upgrade")]
public class HealthBoostUpgradeDefinition : UpgradeDefinition
{
    private void OnEnable()
    {
        if (components.Count == 0)
        {
            HealthBoostComponent healthBoostComponent = new HealthBoostComponent(10);
            components.Add(healthBoostComponent);
        }
    }
}
