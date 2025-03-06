using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int OriginalMaxHealth { get; private set; } = 100;
    public int OriginalMaxArmor { get; private set; } = 0;
    public float OriginalMaxMovementSpeed { get; private set; } = 5f;
    public float OriginalBulletSpeed { get; private set; } = 10f;
    public int OriginalBulletDamage { get; private set; } = 10;

    public int MaxHealth = 100;
    public int MaxArmor = 0;
    public float MaxMovementSpeed = 5f;

    private int healthIncrease = 0;
    private int armorIncrease = 0;
    private int bulletDamageIncrease = 0;
    private int bulletSpeedIncrease = 0;
    private int movementSpeedIncrease = 0;

    [SerializeField] private int armor;
    [SerializeField] private int health;
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float movementSpeed = 5f;

    public int Armor
    {
        get => armor;
        set => armor = value;
    }

    public int Health
    {
        get => health;
        private set => health = value;
    }


    public int BulletDamage
    {
        get => bulletDamage;
        set => bulletDamage = value;
    }

    public float BulletSpeed
    {
        get => bulletSpeed;
        set => bulletSpeed = value;
    }

    public float MovementSpeed
    {
        get => movementSpeed;
        set => movementSpeed = value;
    }

    public void Damage (int damageAmount)
    {
        Health -= damageAmount - Armor;
    }

    public void ResetStats()
    {
        Health = MaxHealth;
        Armor = MaxArmor;
        MaxArmor = OriginalMaxArmor;
        MaxHealth = OriginalMaxHealth;
    }

    public void ApplyStatModifier(StatType statType, int amount)
    {
        switch (statType)
        {
            case StatType.Health:
                healthIncrease += amount;
                break;
            
            case StatType.Armor:
                armorIncrease += amount;
                break;
            
            case StatType.MovementSpeed:
                movementSpeedIncrease += amount;
                break;
            
            case StatType.BulletSpeed:
                bulletSpeedIncrease += amount;
                break;
            
            case StatType.BulletDamage:
                bulletDamageIncrease += amount;
                break;
            
            default:
                throw new Exception("Invalid stat type");
        }

        MaxHealth += healthIncrease;
        MaxArmor += armorIncrease;
        MaxMovementSpeed += movementSpeedIncrease;
        BulletSpeed += bulletSpeedIncrease;
        BulletDamage += bulletDamageIncrease;
    }

    public override string ToString()
    {
        return $"Current stats: " +
            $"Health: {Health}, " +
            $"Armor: {Armor}, " +
            $"BulletDamage: {BulletDamage}, " +
            $"BulletSpeed: {BulletSpeed}, " +
            $"MovementSpeed: {MovementSpeed}" +
            $"\n Max stats: Health: {MaxHealth}, Armor: {MaxArmor}, MovementSpeed: {MaxMovementSpeed}";
    }
}

