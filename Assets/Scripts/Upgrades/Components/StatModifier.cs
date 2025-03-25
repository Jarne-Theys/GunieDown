using UnityEngine;

public class StatModifierComponent : UpgradeComponentBase
{
    [SerializeField]
    private PlayerStatType statType;

    [SerializeField]
    private int increaseAmount;

    public StatModifierComponent() {}
    
    public override void ApplyPassive(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        stats.ApplyStatModifier(statType, increaseAmount);
    }
}
