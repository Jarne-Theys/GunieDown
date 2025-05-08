using UnityEngine;

public class BaseUpgradesList : ScriptableObject
{
    [SerializeReference]
    public UpgradeDefinition[] upgrades;
    
    public UpgradeDefinition GetRandomUpgrade()
    {
        return upgrades[Random.Range(0, upgrades.Length)];
    }
}
