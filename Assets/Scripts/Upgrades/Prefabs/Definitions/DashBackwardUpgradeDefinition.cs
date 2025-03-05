using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "DashBackwardUpgrade", menuName = "Scripts/Upgrades/Dash Backward")]
public class DashBackwardUpgradeDefinition : UpgradeDefinition
{
    private void OnEnable()
    {
        if (components.Count == 0)
        {
            DashBackwardComponent dashBackwardComponent = new DashBackwardComponent(8f);
            components.Add(dashBackwardComponent);
            
            InputActivationComponent inputActivationComponent = new InputActivationComponent(dashBackwardComponent.Activate);
            components.Add(inputActivationComponent);
        }
    }
}
