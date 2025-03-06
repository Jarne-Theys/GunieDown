using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "Scripts/Upgrades/Stats")]
public class StatBoostUpgradeDefinition : UpgradeDefinition
{
    private void OnEnable()
    {
        if (components.Count == 0)
        {
            StatModifierComponent statModifierComponentTemplate = new StatModifierComponent();
            components.Add(statModifierComponentTemplate);
        }
    }

}
