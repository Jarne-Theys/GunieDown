using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradesList upgradesList;
    private List<IUpgradeComponent> activeUpgradeInstances = new List<IUpgradeComponent>();

    /**
     * This is purely meant to be able to iterate over all applied upgrades, for detecting eg duplicates.
     */
    private List<String> activeUpgradeDefinitions = new List<String>();
    /// <summary>
    /// Fires whenever *any* upgrade is applied.
    /// </summary>
    public event Action<UpgradeDefinition, IUpgradeComponent> OnUpgradeAcquired;

    public void AcquireUpgrade(UpgradeDefinition upgradeDefinition)
    {
        if (activeUpgradeDefinitions.Contains(upgradeDefinition.upgradeName))
        {
            Debug.Log("Not re-applying upgrade: " + upgradeDefinition.upgradeName);
            return;
        }
        activeUpgradeDefinitions.Add(upgradeDefinition.upgradeName);
        
        var newComponents = upgradeDefinition.CreateRuntimeComponentsAndConfigure();
        activeUpgradeInstances.AddRange(newComponents);

        foreach (var component in newComponents)
        {
            component.ApplyPassive(gameObject);

            // broadcast *which* definition just gave *which* component
            OnUpgradeAcquired?.Invoke(upgradeDefinition, component);
        }
        
        // Debug.Log("Applied upgrade: " + upgradeDefinition.upgradeName);
    }

    public override string ToString()
    {
        return activeUpgradeInstances.ToString();
    }
}