using UnityEngine;

public class StatModifierComponent : IAbilityComponent
{
    public float HealthMultiplier { get; private set; }
    public float ArmorAddition { get; private set; }
    public float BulletDamageMultiplier { get; private set; }
    public float BulletSpeedMultiplier { get; private set; }
    public float MovementSpeedMultiplier { get; private set; }

    public StatModifierComponent(
        float healthMultiplier = 1f,
        float armorAddition = 0f,
        float bulletDamageMultiplier = 1f,
        float bulletSpeedMultiplier = 1f,
        float movementSpeedMultiplier = 1f)
    {
        HealthMultiplier = healthMultiplier;
        ArmorAddition = armorAddition;
        BulletDamageMultiplier = bulletDamageMultiplier;
        BulletSpeedMultiplier = bulletSpeedMultiplier;
        MovementSpeedMultiplier = movementSpeedMultiplier;
    }

    public void Activate(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        stats.ApplyStatModifier(
            HealthMultiplier,
            ArmorAddition,
            BulletDamageMultiplier,
            BulletSpeedMultiplier,
            MovementSpeedMultiplier);

        Debug.Log("Applied stat modifier");
    }
}
