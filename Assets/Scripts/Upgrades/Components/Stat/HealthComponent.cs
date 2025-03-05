using UnityEngine;

public class HealthComponent : StatModifierComponent
{
    public HealthComponent(int healthIncreaseAmount) : base(StatType.Health, healthIncreaseAmount) { }
    
    public void Activate(GameObject player) { }

    public void ApplyPassive(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.ApplyStatModifier(StatType.Health, increaseAmount);
    }
}