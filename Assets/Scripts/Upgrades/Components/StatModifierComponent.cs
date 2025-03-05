using UnityEngine;

public class StatModifierComponent : UpgradeComponentBase
{
    public StatType statType;
    public int increaseAmount;

    public StatModifierComponent(StatType statType, int amount)
    {
        this.statType = statType;
        this.increaseAmount = amount;
    }

    public override void Activate(GameObject player) { }
    
    public override void ApplyPassive(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        stats.ApplyStatModifier(statType, increaseAmount);
    }
}
