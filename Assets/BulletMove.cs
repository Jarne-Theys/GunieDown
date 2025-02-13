using System.Collections;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private GameObject hitParticleSystemPrefab;
    public float bulletSpeed;
    public int bulletDamage;
    void Start()
    {
        BulletStats bulletStats = GetComponent<BulletStats>();
        bulletSpeed = bulletStats.Speed;
        bulletDamage = bulletStats.Damage;
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

        if (collision.gameObject.TryGetComponent<PlayerStats>(out var otherPlayer))
        {
            // Handle collision with PlayerStats object
            if (!otherPlayer || otherPlayer.tag != "Player")
            {
                return;
            } else
            {
                Debug.Log("Hit player");
                otherPlayer.GetComponent<PlayerStats>().Damage(bulletDamage);
            }

        }

        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }
}
