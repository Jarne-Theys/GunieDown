using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public int OriginalMaxAmmo { get; private set; } = 30;
    public int OriginalFireRate { get; private set; } = 120;
    public float OriginalReloadTime { get; private set; } = 1.5f;

    public int MaxAmmo = 30;
    public int FireRate = 120;
    public float ReloadTime = 1.5f;
    public int CurrentAmmo = 30;

    private int maxAmmoIncrease = 0;
    private int fireRateIncrease = 0;
    private float reloadTimeDecrease = 0;

    public void ResetStats()
    {
        MaxAmmo = OriginalMaxAmmo;
        FireRate = OriginalFireRate;
        ReloadTime = OriginalReloadTime;
    }

    public void ApplyStatModifier(WeaponStatType statType, int value)
    {
        switch (statType)
        {
            case WeaponStatType.MaxAmmo:
                maxAmmoIncrease += value;
                break;
            case WeaponStatType.FireRate:
                fireRateIncrease += value;
                break;
            case WeaponStatType.ReloadTime:
                reloadTimeDecrease += value;
                break;

            default:
                throw new System.Exception("Invalid stat type");
        }

        MaxAmmo += maxAmmoIncrease;
        FireRate += fireRateIncrease;
        ReloadTime += reloadTimeDecrease;
    }

    public override string ToString()
    {
        return $"Max Ammo: {MaxAmmo}," +
            $"Fire Rate: {FireRate}," +
            $"Reload Time: {ReloadTime}" +
            $"Current Ammo: {CurrentAmmo}";
    }
}
