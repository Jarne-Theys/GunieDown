using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradesList upgradesList;
    private List<IUpgradeComponent> activeUpgradeInstances = new List<IUpgradeComponent>();

    public void AcquireUpgrade(UpgradeDefinition upgradeDefinition)
    {
        // Create brand-new components each time, so delegates or references
        // aren’t lost by Unity’s serialization.
        var newComponents = upgradeDefinition.CreateRuntimeComponents();

        activeUpgradeInstances.AddRange(newComponents);

        foreach (var component in newComponents)
        {
            component.ApplyPassive(gameObject);
        }

        Debug.Log($"Acquired and applied upgrade: {upgradeDefinition.upgradeName}");
    }
}
