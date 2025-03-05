using UnityEngine;

public class StatModifierComponent : IUpgradeComponent
{
    public StatType statType;
    public int increaseAmount;

    public StatModifierComponent(StatType statType, int amount)
    {
        this.statType = statType;
        this.increaseAmount = amount;
    }

    public void Activate(GameObject player) { }
    
    public void ApplyPassive(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        stats.ApplyStatModifier(statType, increaseAmount);
    }
}
