using UnityEngine;

public class HitDetectionAI : MonoBehaviour
{
    private AIPlayerAgent agent;

    private void Start()
    {
        agent = gameObject.GetComponentInParent<AIPlayerAgent>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            ProjectileStats projectileStats = collision.gameObject.GetComponent<ProjectileStats>();
            if (projectileStats == null)
            {
                Debug.LogError("ProjectileStats component not found on bullet. This is likely an error");
            }
            else
            {
                agent.AddExternalReward(-0.5f);
            }
        }
    }
}
