using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ProjectileComponentBase : UpgradeComponentBase, IProjectileComponent
{
    [Header("Projectile settings")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileSpeed = 0f;
    [SerializeField] protected int projectileDamage = 0;
    [SerializeField] protected float projectileLifeTime = 1f;

    [Header("Weapon settings")]
    public int MaxAmmo => weaponMaxAmmo;
    public float ReloadTime => reloadTime;
    public float FireRate  => fireRate;
    
    [SerializeField] private int weaponMaxAmmo = 30;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float fireRate = 240f;
    
    [SerializeField] private int currentAmmo = 30;
    public int CurrentAmmo
    {
        get => currentAmmo;
        set
        {
            if (currentAmmo == value) return;
            currentAmmo = value;
            OnAmmoChanged?.Invoke(currentAmmo, weaponMaxAmmo);
        }
    }

    /**
     * current / max
     */
    public event Action<int,int> OnAmmoChanged;
    
    /**
     * Define the reload action seperately, as all weapons should have the ability to reload themselves.
     * This can be null!
     */
    [SerializeField] private InputActionReference reloadAction;
    private float nextFireTime = 0f;
    private bool  isReloading  = false;
    
    public Vector3[] LastProjectilePositions { get; set; }
    
    public override void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        if (isReloading) return;
        
        if (Time.time < nextFireTime) return;
        
        if (CurrentAmmo <= 0)
        {
            BeginReload();
            return;
        }

        ExecuteActivation(player, runtimeComponents);

        CurrentAmmo--;
        nextFireTime = Time.time + (60f / fireRate);
    }
    
    private void BeginReload()
    {
        isReloading = true;
        CoroutineRunner.Instance.StartCoroutine(ReloadCoroutine());
    }
    

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        CurrentAmmo = weaponMaxAmmo;
        isReloading  = false;
    }
    
    protected Transform GetWeaponTransform(GameObject player)
    {
        Transform weapon = player.transform.Find("Weapon");
        if (weapon == null)
        {
            Debug.LogError($"The weapon child gameobject was not found on the player {player.name}");
        }
        return weapon;
    }

    public override void ApplyPassive(GameObject player)
    {
        if (reloadAction != null)
        {
            reloadAction.action.performed += ctx => BeginReload();
        }
        
        OnAmmoChanged?.Invoke(currentAmmo, weaponMaxAmmo);
    }

    ~ProjectileComponentBase()
    {
        if (reloadAction != null)
        {
            reloadAction.action.performed -= ctx => BeginReload();
        }
    }
}
