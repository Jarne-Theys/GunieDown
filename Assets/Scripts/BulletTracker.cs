using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletTracker : MonoBehaviour
{
    public float detectionRadius = 10f; // How far the AI can "see" bullets
    public int maxTrackedBullets = 5;   // Max bullets AI can track at a time
    public LayerMask bulletLayer;       // Only check bullet layer for performance

    public List<Transform> trackedBullets = new List<Transform>();

    public void DetectBullets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, bulletLayer);

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

    public void ClearTrackedBulletList()
    {
        trackedBullets.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
