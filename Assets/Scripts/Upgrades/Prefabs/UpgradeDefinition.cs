using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "Scriptable Objects/UpgradeDefinition")]
public abstract class UpgradeDefinition : ScriptableObject
{
    public string upgradeName;
    public string description;
    
    [SerializeReference]
    public List<IUpgradeComponent> components = new List<IUpgradeComponent>();

    public void ApplyUpgrade(GameObject player)
    {
        foreach (var component in components)
        {
            component.ApplyPassive(player);
        }
    }

    private void OnEnable()
    {
        if (components == null)
            components = new List<IUpgradeComponent>();
    }
}
