using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int MaxHealth = 100;
    public int MaxArmor = 0;

    [SerializeField] private int armor;

    public int Armor
    {
        get => armor;
        set => armor = value;
    }

    [SerializeField] private int health;
    public int Health
    {
        get => health;
        private set => health = value;
    }


    [SerializeField] private int bulletDamage = 10;
    public int BulletDamage
    {
        get => bulletDamage;
        set => bulletDamage = value;
    }

    [SerializeField] private float bulletSpeed = 10f;
    public float BulletSpeed
    {
        get => bulletSpeed;
        set => bulletSpeed = value;
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
}
