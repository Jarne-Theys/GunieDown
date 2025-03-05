using UnityEngine;

public class HealthBoostComponent : StatModifierComponent
{
    public HealthBoostComponent(int healthIncreaseAmount) : base(StatType.Health, healthIncreaseAmount) { }
    
    public override void Activate(GameObject player) { }

    public override void ApplyPassive(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.ApplyStatModifier(StatType.Health, increaseAmount);
    }
}