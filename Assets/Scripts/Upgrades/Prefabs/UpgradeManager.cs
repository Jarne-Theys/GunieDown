using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradesList upgradesList;
    private List<IUpgradeComponent> activeUpgradeInstances = new List<IUpgradeComponent>();

    public void AcquireUpgrade(UpgradeDefinition upgradeDefinition)
    {
        // Use the new method that creates and configures components
        var newComponents = upgradeDefinition.CreateRuntimeComponentsAndConfigure();

        activeUpgradeInstances.AddRange(newComponents);

        foreach (var component in newComponents)
        {
            component.ApplyPassive(gameObject);
        }

    }

    public override string ToString()
    {
        return activeUpgradeInstances.ToString();
    }
}