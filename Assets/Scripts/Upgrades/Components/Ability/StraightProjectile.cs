using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StraightProjectile : ProjectileComponentBase
{
    public StraightProjectile() {}

    protected override void ExecuteActivation(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        Transform weapon = player.transform.Find("Weapon");
        if (weapon == null)
        {
            Debug.LogError($"The weapon child gameobject was not found on the player {player.name}");
        }
        
        //TODO: USE WEAPON INSTEAD OF PLAYER FORWARD FOR DIRECTION
        
        Vector3 shootDirection = player.transform.forward;
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection, Vector3.up);

        GameObject projectile = GameObject.Instantiate(projectilePrefab, player.transform.position + shootDirection * 1f, shootRotation);

        var projectileStats = projectile.GetComponent<ProjectileStats>();
        projectileStats.Damage = projectileDamage;
        projectileStats.Speed = projectileSpeed;

        LastProjectilePositions = new Vector3[] { projectile.transform.position};

        GameObject.Destroy(projectile, projectileLifeTime);
    }
}
