using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = gameObject.GetComponentInParent<PlayerStats>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Player hit by bullet");
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
