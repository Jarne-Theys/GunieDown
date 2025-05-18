using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletMove : MonoBehaviour
{
    float bulletSpeed;
    int bulletDamage;


    [SerializeField]
    private bool destroyOnPlayerContact;

    [SerializeField]
    private bool destroyOnTerrainContact;

    private Rigidbody rb;
    
    public bool addRewardToAgentOnTargetHit = false;
    public bool subtractRewardFromAgentOnAgentHit = false;
    public bool subtractRewardFromAgentOnMiss = false;
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

    public void Init(AIPlayerAgent aiAgent, bool rewardAgentForHitting, bool punishAgentForGettingHit, bool punishAgentForMiss)
    {
        this.agent = aiAgent;
        this.addRewardToAgentOnTargetHit = rewardAgentForHitting;
        this.subtractRewardFromAgentOnAgentHit = punishAgentForGettingHit;
        this.subtractRewardFromAgentOnMiss = punishAgentForMiss;
        
        var bulletCollision = GetComponentInChildren<BulletCollision>();
        
        bulletCollision.subtractRewardFromAgentOnMiss = this.subtractRewardFromAgentOnMiss;
        bulletCollision.destroyOnTerrainContact = this.destroyOnTerrainContact;
        
        bulletCollision.addRewardToAgentOnTargetHit = this.addRewardToAgentOnTargetHit;
        bulletCollision.subtractRewardFromAgentOnAgentHit = this.subtractRewardFromAgentOnAgentHit;
        bulletCollision.destroyOnPlayerContact = this.destroyOnPlayerContact;
        
        bulletCollision.bulletDamage = this.bulletDamage;
        
        bulletCollision.agent = aiAgent;
    }
    

    void OnDestroy()
    {
        // if (subtractRewardFromAgentOnMiss)
        // {
        //     agent.AddExternalReward(-0.1f, "Punished for miss");
        // }
    }
}
