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
    
    public bool addRewardToAgent = false;
    public bool subtractRewardFromAgent = false;
    public AIPlayerAgent agent;


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

    public void Init(AIPlayerAgent aiAgent, bool rewardAgentForHitting, bool punishAgentForGettingHit)
    {
        this.agent = aiAgent;
        this.addRewardToAgent = rewardAgentForHitting;
        this.subtractRewardFromAgent = punishAgentForGettingHit;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            if (destroyOnTerrainContact)
            {
                if (addRewardToAgent)
                {
                    agent.AddExternalReward(-0.01f);
                }
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            // TODO: make this method deal damage to the player hit
            if (destroyOnPlayerContact)
            {
                if (addRewardToAgent)
                {
                    // Increase from 1 to 10 for more motivation
                    agent.AddExternalReward(10f);
                    agent.EndEpisodeExternal("AI hit player!");
                }
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("AIPlayer"))
        {
            if (destroyOnPlayerContact)
            {
                if (subtractRewardFromAgent)
                {
                    agent.AddExternalReward(-0.1f);
                }
                Destroy(gameObject);
            }
        }

        else
        {
            Debug.LogWarning($"Bullet collided with something that's not a 'Wall', 'Player' or 'AIPlayer': \n {collision.gameObject.name}");
            Destroy(gameObject);
        }
        
    }

    void OnDestroy()
    {
        
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

            if (other.gameObject.layer == LayerMask.NameToLayer("Wall") && destroyOnTerrainContact)
            {
                Debug.Log("Hit terrain!");
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
