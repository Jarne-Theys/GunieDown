using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradesList upgradesList;
    private List<IUpgradeComponent> activeUpgradeInstances = new List<IUpgradeComponent>();

    /// <summary>
    /// Fires whenever *any* upgrade is applied.
    /// </summary>
    public event Action<UpgradeDefinition, IUpgradeComponent> OnUpgradeAcquired;

    public void AcquireUpgrade(UpgradeDefinition upgradeDefinition)
    {
        var newComponents = upgradeDefinition.CreateRuntimeComponentsAndConfigure();
        activeUpgradeInstances.AddRange(newComponents);

        foreach (var component in newComponents)
        {
            component.ApplyPassive(gameObject);

            // broadcast *which* definition just gave *which* component
            OnUpgradeAcquired?.Invoke(upgradeDefinition, component);
        }
    }

    public override string ToString()
    {
        return activeUpgradeInstances.ToString();
    }
}