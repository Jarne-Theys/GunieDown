using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CreateDamageZone : UpgradeComponentBase
{
    [SerializeField]
    private GameObject damageZonePrefab;

    [SerializeField]
    private float damageZoneDuration = 1f;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private Vector3 spawnPositionOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("If true, the damage zone will be spawned at the position(s) of any projectile(s) fired when they despawn. If false, use player position.")]
    private bool useProjectilePosition = false;

    public CreateDamageZone() { }



    public override void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        Vector3[] spawnPositions = new Vector3[] { player.transform.position };

        if (useProjectilePosition)
        {
            // Try to find a CreateStraightProjectile component in the runtime components
            var projectileComponent = runtimeComponents.OfType<IProjectileComponent>().FirstOrDefault();

            if (projectileComponent != null)
            {
                // If CreateStraightProjectile is found, use its projectile position
                spawnPositions = projectileComponent.LastProjectilePositions;
                Debug.Log("DamageZone spawning at projectile position.");
            }
            else
            {
                Debug.Log("CreateStraightProjectile component not found. DamageZone spawning at player position.");
            }
        }

        foreach (var position in spawnPositions)
        {
            GameObject damageZone = GameObject.Instantiate(damageZonePrefab, position + spawnPositionOffset, Quaternion.identity);
            GameObject.Destroy(damageZone, damageZoneDuration);
        }

    }
}
