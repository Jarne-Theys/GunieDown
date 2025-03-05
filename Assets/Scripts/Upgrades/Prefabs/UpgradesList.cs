using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesList", menuName = "Scripts/Upgrades List")]
public class UpgradesList : ScriptableObject
{
    [SerializeReference]
    public UpgradeDefinition[] upgrades;
    
    public UpgradeDefinition GetRandomUpgrade()
    {
        return upgrades[Random.Range(0, upgrades.Length)];
    }
}
