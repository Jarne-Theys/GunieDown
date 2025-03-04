using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public string Name { get; set; }
    public List<IAbilityComponent> Components { get; private set; } = new List<IAbilityComponent>();

    public Ability(string name)
    {
        Name = name;
    }

    public void AddComponent(IAbilityComponent component)
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
}
