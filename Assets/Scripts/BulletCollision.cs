using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [HideInInspector]
    public int bulletDamage;
    public bool addRewardToAgentOnTargetHit = false;
    public bool subtractRewardFromAgentOnAgentHit = false;
    public bool subtractRewardFromAgentOnMiss = false;

    public bool destroyOnPlayerContact;
    public bool destroyOnTerrainContact;
    
    public AIPlayerAgent agent;
    
    void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            Debug.LogError("other is null");
        }

        else if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Wall"))
        {
            if (destroyOnTerrainContact)
            {
                if (subtractRewardFromAgentOnMiss)
                {
                    agent.AddExternalReward(-0.1f, "Punished for hitting terrain");
                }
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
        
        else if (other.gameObject.CompareTag("Player"))
        {
            // TODO: make this method deal damage to the player hit
            if (destroyOnPlayerContact)
            {
                if (addRewardToAgentOnTargetHit)
                {
                    agent.AddExternalReward(5f, "Rewarded for hitting player");
                    agent.EndEpisodeExternal("Hitting target!");
                }

                else
                {
                    PlayerStats playerStats = other.gameObject.GetComponent<PlayerStats>();
                    playerStats.Damage(bulletDamage); 
                }
                
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("AIPlayer"))
        {
            if (destroyOnPlayerContact)
            {
                if (subtractRewardFromAgentOnAgentHit)
                {
                    agent.AddExternalReward(-0.5f, "Punished for getting hit");
                }

                else
                {
                    PlayerStats playerStats = other.gameObject.GetComponent<PlayerStats>();
                    playerStats.Damage(bulletDamage);
                }
                
                Destroy(gameObject.transform.parent.gameObject);
            }
        }

        else
        {
            Debug.Log($"Hit something else than a player or terrain ({other.gameObject.name}), is this intended?");
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
