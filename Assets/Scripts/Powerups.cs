using System;
using UnityEngine;

public abstract class Upgrade
{
    public string Name { get; protected set; } = "Powerup name";
    public string Description { get; protected set; } = "Powerup description";
    public abstract void Apply(GameObject player);
}


public abstract class Powerup : Upgrade
{
    public float HealthMultiplier { get; protected set; } = 1f;
    public float ArmorAddition { get; protected set; } = 0f;
    public float BulletDamageMultiplier { get; protected set; } = 1f;
    public float BulletSpeedMultiplier { get; protected set; } = 1f;
    public float MovementSpeedMultiplier { get; protected set; } = 1f;

    public override void Apply(GameObject player)
    {
        player.GetComponent<PlayerStats>().ApplyPowerup(this);
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

class BulletDamageBoost : Powerup
{
    public BulletDamageBoost()
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