using UnityEngine;

public abstract class Powerup
{
    public string Name { get; protected set; } = "Powerup name";
    public string Description { get; protected set; } = "Powerup description";

    public float HealthMultiplier { get; protected set; } = 1;
    public float ArmorAddition { get; protected set; } = 01;
    public float BulletDamageMultiplier { get; protected set; } = 1;
    public float BulletSpeedMultiplier { get; protected set; } = 1f;
    public float MovementSpeedMultiplier { get; protected set; } = 1f;

    public virtual void Apply(PlayerStats player)
    {
        player.MaxHealth = (int)(player.OriginalMaxHealth * HealthMultiplier);
        player.MaxArmor = (int)(player.OriginalMaxArmor + ArmorAddition);
        player.MaxMovementSpeed = player.OriginalMaxMovementSpeed * MovementSpeedMultiplier;

        player.BulletDamage = (int)(player.OriginalBulletDamage * BulletDamageMultiplier);
        player.BulletSpeed = player.OriginalBulletSpeed * BulletSpeedMultiplier;
    }
}

class HealthBoost : Powerup
{
    public HealthBoost()
    {
        Name = "Health";
        Description = "Increases your total health, so you can take more damage";
        HealthMultiplier = 1.1f;
    }
}

class DamageBoost : Powerup
{
    public DamageBoost()
    {
        Name = "Damage";
        Description = "Increases the damage of your bullets";
        BulletDamageMultiplier = 1.25f;
    }
}

class BulletSpeedBoost : Powerup
{
    public BulletSpeedBoost()
    {
        Name = "Bullet speed";
        Description = "Increases the travel speed of your bullets";
        BulletSpeedMultiplier = 1.25f;
    }
}

class ArmorBoost : Powerup
{
    public ArmorBoost()
    {
        Name = "Armor";
        Description = "Increases your armor, so you take less damage from everything";
        ArmorAddition = 3;
    }
}