using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletTracker : MonoBehaviour
{
    public float detectionRadius = 10f; // How far the AI can "see" bullets
    public int maxTrackedBullets = 5;   // Max bullets AI can track at a time
    public LayerMask bulletLayer;       // Only check bullet layer for performance

    public static List<Transform> trackedBullets = new List<Transform>();

    void Update()
    {
        DetectBullets();
    }

    void DetectBullets()
    {
        // Find nearby bullets using a physics sphere
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, bulletLayer);

        // Convert colliders to a list of bullet transforms
        List<Transform> bullets = new List<Transform>();
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Bullet"))
            {
                bullets.Add(collider.transform);
            }
        }

        // Sort bullets by distance (closest first)
        bullets = bullets.OrderBy(b => Vector3.Distance(transform.position, b.position)).ToList();

        // Store the closest ones (limit to maxTrackedBullets)
        trackedBullets = bullets.Take(maxTrackedBullets).ToList();
    }

    // For Debugging in Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
