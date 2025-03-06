using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "DashBackwardUpgrade", menuName = "Scripts/Upgrades/Dash Backward")]
public class DashBackwardUpgradeDefinition : UpgradeDefinition
{
    private void OnEnable() // This shows the components in the editor
    {
        if (components.Count == 0)
        {
            DashBackwardComponent dashComponentTemplate = new DashBackwardComponent();
            components.Add(dashComponentTemplate);

            InputActivationComponent inputComponentTemplate = new InputActivationComponent(); 
            components.Add(inputComponentTemplate);
        }
    }

    public override List<IUpgradeComponent> CreateRuntimeComponents()
    {
        List<IUpgradeComponent> runtimeComponents = base.CreateRuntimeComponents();

        DashBackwardComponent runtimeDash = runtimeComponents.OfType<DashBackwardComponent>().FirstOrDefault();
        InputActivationComponent runtimeInput = runtimeComponents.OfType<InputActivationComponent>().FirstOrDefault();

        if (runtimeDash != null && runtimeInput != null)
        {
            runtimeInput.onActivate = runtimeDash.Activate;
        }

        return runtimeComponents;
    }
}