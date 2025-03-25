using System.Collections;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private GameObject hitParticleSystemPrefab;


    float bulletSpeed;
    int bulletDamage;

    float fallRate;
    float currentFallRate;

    private void Awake()
    {
    }

    void Start()
    {
        ProjectileStats projectileStats = GetComponent<ProjectileStats>();
        bulletSpeed = projectileStats.Speed;
        bulletDamage = projectileStats.Damage;
        if (projectileStats.GetType() == typeof(GravityProjectileStats))
        {
            // Don't use "as" here, it will return null if the cast fails
            // Manually casting so an error gets thrown if projectileStats is not a GravityProjectileStats. This should never happen.
            GravityProjectileStats gravityProjectileStats = (GravityProjectileStats)projectileStats;
            fallRate = gravityProjectileStats.FallRate * 0.01f;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AIPlayer"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.Damage(bulletDamage);
            }

            AIPlayerAgent aIPlayer = other.GetComponent<AIPlayerAgent>();
            if (aIPlayer != null)
            {
                aIPlayer.AddExternalReward(-10);
            }
        }

        BulletTracker.trackedBullets.Remove(transform);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * bulletSpeed * Time.fixedDeltaTime;

        currentFallRate += fallRate * Time.fixedDeltaTime;
        transform.position += Vector3.down * currentFallRate;
    }
}
