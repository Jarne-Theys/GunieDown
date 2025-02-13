using UnityEngine;

public abstract class Powerup
{
    public float HealthMultiplier { get; protected set; } = 1;
    public float ArmorMultiplier { get; protected set; } = 1;
    public float BulletDamageMultiplier { get; protected set; } = 1;
    public float BulletSpeedMultiplier { get; protected set; } = 1f;
    public float MovementSpeedMultiplier { get; protected set; } = 1f;

    public virtual void Apply(PlayerStats player)
    {
        player.MaxHealth = (int)(player.OriginalMaxHealth * HealthMultiplier);
        player.MaxArmor = (int)(player.OriginalMaxArmor * ArmorMultiplier);
        player.MaxMovementSpeed = player.OriginalMaxMovementSpeed * MovementSpeedMultiplier;

        player.BulletDamage = (int)(player.OriginalBulletDamage * BulletDamageMultiplier);
        player.BulletSpeed = player.OriginalBulletSpeed * BulletSpeedMultiplier;
    }
}

class HealthBoost : Powerup
{
    public HealthBoost()
    {
        HealthMultiplier = 1.1f;
    }
}

