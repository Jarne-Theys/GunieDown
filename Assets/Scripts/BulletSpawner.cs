using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform target;

    [Range(1f, 20f)]
    public float bulletSpeed;

    [Range(0, 50)]
    public int bulletDamage;

    private float timer = 0f;
    public float waitTime = 3f;

    public AIPlayerAgent aiAgent;
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.transform.LookAt(target);
            ProjectileStats projectileStats = bullet.GetComponent<ProjectileStats>();
            projectileStats.Speed = bulletSpeed;
            projectileStats.Damage = bulletDamage;
            timer = 0f;
            
            BulletMove projectileMover = bullet.GetComponent<BulletMove>();
            projectileMover.Init(aiAgent,false, true);
        }
    }
}
