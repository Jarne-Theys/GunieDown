using System.Collections;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private GameObject hitParticleSystemPrefab;
    float bulletSpeed;
    int bulletDamage;
    void Start()
    {
        // TODO: replace this so it handles both projectile stats and gravity projectile stats
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

        /*
        TODO: Reimplement player hit detection
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponentInChildren<PlayerStats>();
            playerStats.Damage(bulletDamage);
        }
        */
        // Apply reward to AI Agent

        StartCoroutine(DestroyBullet());
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(0.1f);
        BulletTracker.trackedBullets.Remove(transform);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }
}
