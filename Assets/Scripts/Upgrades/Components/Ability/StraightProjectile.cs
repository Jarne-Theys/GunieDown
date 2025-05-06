using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StraightProjectile : ProjectileComponentBase
{
    [Header("Agent settings")]
    public bool rewardAgentForHittingTarget;
    public bool punishAgentForGettingHit;
    public bool punishAgentForMiss;
    
    public StraightProjectile() {}

    protected override void ExecuteActivation(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        var weaponTransform = GetWeaponTransform(player);
        
        Vector3 shootDirection = weaponTransform.forward;
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection, Vector3.up);

        GameObject projectile = GameObject.Instantiate(projectilePrefab, player.transform.position + shootDirection * 1f, shootRotation);

        var projectileStats = projectile.GetComponent<ProjectileStats>();
        projectileStats.Damage = projectileDamage;
        projectileStats.Speed = projectileSpeed;
        
        // Make sure the bullet can't hit the person that fired it
        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();

        Physics.IgnoreCollision(projectileCollider, playerCollider);

        if (player.CompareTag("AIPlayer"))
        {
            BulletMove projectileMover = projectile.GetComponent<BulletMove>();
            AIPlayerAgent playerAgent = player.gameObject.GetComponent<AIPlayerAgent>();
            projectileMover.Init(playerAgent,rewardAgentForHittingTarget, punishAgentForGettingHit, punishAgentForMiss);
        }
        
        

        LastProjectilePositions = new Vector3[] { projectile.transform.position};

        GameObject.Destroy(projectile, projectileLifeTime);
    }
}
