using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GravityProjectile : ProjectileComponentBase
{
    [SerializeField]
    private float fallRate = 0.1f;

    public GravityProjectile() { }

    public override void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        Vector3 shootDirection = player.transform.forward;
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection, Vector3.up);

        GameObject projectile = GameObject.Instantiate(projectilePrefab, player.transform.position + shootDirection * 1f, shootRotation);

        var projectileStats = projectile.GetComponent<GravityProjectileStats>();
        projectileStats.Damage = projectileDamage;
        projectileStats.Speed = projectileSpeed;
        projectileStats.FallRate = fallRate;

        CoroutineRunner.Instance.StartCoroutine(DestroyDelayed(projectile));
    }

    private IEnumerator DestroyDelayed(GameObject projectile)
    {
        yield return new WaitForSeconds(projectileLifeTime);
        GameObject.Destroy(projectile);
        LastProjectilePositions = new Vector3[] { projectile.transform.position };
    }
}
