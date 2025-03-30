using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CreateDamageZone : UpgradeComponentBase
{
    [SerializeField]
    private GameObject damageZonePrefab;

    [SerializeField]
    private float damageZoneDuration = 0.1f;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private Vector3 spawnPositionOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("If true, the damage zone will be spawned at the position(s) of any projectile(s) fired when they despawn. If false, use player position.")]
    private bool useProjectilePosition = false;

    [SerializeField]
    [Tooltip("Optionally wait x seconds before triggering this component.")]
    private float activationDelay = 0f;

    public CreateDamageZone() { }



    public override void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        CoroutineRunner.Instance.StartCoroutine(ActivateDelayed(player, runtimeComponents));
    }

    private IEnumerator ActivateDelayed(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        yield return new WaitForSeconds(activationDelay);
        Vector3[] spawnPositions;

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
                throw new MissingFieldException("No projectile position found, but use projectile position set to true. Set use projectile position to false or add a projectile component.");
            }
        }
        else
        {
            spawnPositions = new Vector3[] { player.transform.position };
            Debug.Log("Use projectile position set to false. DamageZone spawning at player position.");
        }

        foreach (var position in spawnPositions)
        {
            GameObject damageZone = GameObject.Instantiate(damageZonePrefab, position + spawnPositionOffset, Quaternion.identity);
            GameObject.Destroy(damageZone, damageZoneDuration);
        }
    }
}
