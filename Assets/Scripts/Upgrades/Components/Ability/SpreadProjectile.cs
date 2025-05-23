using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpreadProjectile : ProjectileComponentBase
{
    [SerializeField]
    private int projectileCount = 10;

    [SerializeField]
    private float spreadAngle = 10f;

    public SpreadProjectile() {}

    protected override void ExecuteActivation(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        var weaponTransform = GetWeaponTransform(player);
        
        Vector3 shootDirection = weaponTransform.forward;
        
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection, Vector3.up);
        for (int i = 0; i <= projectileCount; i++)
        {
            Quaternion spreadRotation = Quaternion.Euler(UnityEngine.Random.Range(-spreadAngle, spreadAngle), UnityEngine.Random.Range(-spreadAngle, spreadAngle), UnityEngine.Random.Range(-spreadAngle, spreadAngle));
            Vector3 spreadDirection = spreadRotation * shootDirection;
            GameObject projectile = GameObject.Instantiate(projectilePrefab, player.transform.position + spreadDirection * 1f, shootRotation * spreadRotation);
            var projectileStats = projectile.GetComponent<ProjectileStats>();
            projectileStats.Damage = projectileDamage;
            projectileStats.Speed = projectileSpeed;
            GameObject.Destroy(projectile, projectileLifeTime);
        }
    }
}
