using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileComponentBase : UpgradeComponentBase, IProjectileComponent
{
    [SerializeField]
    protected GameObject projectilePrefab;

    [SerializeField]
    protected float projectileSpeed = 0f;

    [SerializeField]
    protected int projectileDamage = 0;

    [SerializeField]
    protected float projectileLifeTime = 1f;

    [SerializeField]
    protected float weaponMaxAmmo = 30f;

    [SerializeField] 
    protected float fireRate = 240f;

    [SerializeField]
    protected float reloadTime = 1f;

    [SerializeField] 
    protected float currentAmmo = 30f;
    
    public Vector3[] LastProjectilePositions { get; set; }
}
