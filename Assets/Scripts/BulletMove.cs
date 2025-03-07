using System.Collections;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private GameObject hitParticleSystemPrefab;
    float bulletSpeed;
    int bulletDamage;
    void Start()
    {
        ProjectileStats projectileStats = GetComponent<ProjectileStats>();
        bulletSpeed = projectileStats.Speed;
        bulletDamage = projectileStats.Damage;
    }

    void OnCollisionEnter(Collision collision)
    {
        /*
        GameObject hitParticleSystemGO = Instantiate(hitParticleSystemPrefab, transform.position, transform.rotation);
        ParticleSystem hitParticleSystem = hitParticleSystemGO.GetComponent<ParticleSystem>();
        float hitParticleDuration = hitParticleSystem.main.startLifetimeMultiplier;
        hitParticleSystem.Play();
        Destroy(hitParticleSystemGO, hitParticleDuration);
        */

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponentInChildren<PlayerStats>();
            playerStats.Damage(bulletDamage);
        }

        // Apply reward to AI Agent
        if (collision.gameObject.TryGetComponent<AIPlayerAgent>(out var aIPlayer))
        {
            if (!aIPlayer || aIPlayer.tag != "AIPlayer")
            {
                return;
            }
            else
            {
                aIPlayer.AddExternalReward(-0.1f);
            }
        }

        BulletTracker.trackedBullets.Remove(transform);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }
}
