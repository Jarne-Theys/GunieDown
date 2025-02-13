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
    }

    public void ApplyPowerup(Powerup powerup)
    {
        powerup.Apply(this);
    }

    public override string ToString()
    {
        return $"Current stats: Health: {Health}, Armor: {Armor}, BulletDamage: {BulletDamage}, BulletSpeed: {BulletSpeed}, MovementSpeed: {MovementSpeed}" +
            $"\n Max stats: Health: {MaxHealth}, Armor: {MaxArmor}, MovementSpeed: {MaxMovementSpeed}";
    }
}
