using System.Collections;
using UnityEngine;

public class BulletMove : MonoBehaviour
{


    float bulletSpeed;
    int bulletDamage;


    [SerializeField]
    private bool destroyOnPlayerContact;

    [SerializeField]
    private bool destroyOnTerrainContact;

    private Rigidbody rb;

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
            //fallRate = gravityProjectileStats.FallRate * 0.01f;
        }

        rb = GetComponent<Rigidbody>();
        //rb.linearVelocity = transform.forward * bulletSpeed;
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (destroyOnTerrainContact)
            {
                Destroy(gameObject);
            }
            // Otherwise, let the Physics Material on the collider handle the bounce.
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (destroyOnPlayerContact)
            {
                Destroy(gameObject);
            }
        }
    }

    /*    public void OnTriggerEnter(Collider other)
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
                    aIPlayer.AddExternalReward(-1f);
                }

                // Training
                MockPlayerMover mockPlayerMover = other.GetComponent<MockPlayerMover>();
                if (mockPlayerMover != null)
                {
                    aIPlayer.AddExternalReward(1f);
                }


                if (destroyOnPlayerContact)
                {
                    BulletTracker.trackedBullets.Remove(transform);
                    Destroy(gameObject);
                }

                else
                {
                    // To avoid damaging the target multiple times.
                    bulletDamage = 0;
                }
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") && destroyOnTerrainContact)
            {
                Debug.Log("Hit ground!");
                BulletTracker.trackedBullets.Remove(transform);
                Destroy(gameObject);
            }
        }*/

    /*    private void FixedUpdate()
        {
            Vector3 movement = transform.forward * bulletSpeed;

            if (fallRate > 0f)
            {
                currentFallRate += fallRate * Time.fixedDeltaTime;
                movement += Vector3.down * currentFallRate;
            }


            rb.linearVelocity = movement;

        }*/
}
