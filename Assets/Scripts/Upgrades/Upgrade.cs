using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    public string Name { get; set; }
    public List<IUpgradeComponent> Components { get; private set; } = new List<IUpgradeComponent>();

    public Upgrade(string name)
    {
        Name = name;
    }
    
    public void AddComponent(IUpgradeComponent component)
    {
        Components.Add(component);
    }

    public void Activate(GameObject player)
    {
        foreach (var component in Components)
        {
            component.Activate(player);
        }
    }
    
    public void ApplyPassiveEffects(GameObject player)
    {
        foreach (var component in Components)
        {
            component.ApplyPassive(player);
        }
    }
}
