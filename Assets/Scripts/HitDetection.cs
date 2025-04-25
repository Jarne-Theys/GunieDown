using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private bool addRewardToAgent;

    [SerializeField] private AIPlayerAgent agent;

    private void Start()
    {
        playerStats = gameObject.GetComponentInParent<PlayerStats>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // set this to true during training
            if (addRewardToAgent)
            {
                if (gameObject.CompareTag("Player"))
                {
                    agent.AddExternalReward(1f);
                    Debug.Log("AI hit (mock) player!");
                }
                else
                {
                    agent.AddExternalReward(-0.1f);
                }
                return;
            }
            
            ProjectileStats projectileStats = collision.gameObject.GetComponent<ProjectileStats>();
            if (projectileStats == null)
            {
                Debug.LogError("ProjectileStats component not found on bullet. This is likely an error");
            }
            else
            {
                playerStats.Damage(projectileStats.Damage);
            }
        }
    }
}
