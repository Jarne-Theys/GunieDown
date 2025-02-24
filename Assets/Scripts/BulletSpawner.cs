using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform target;

    [Range(1f, 20f)]
    public float bulletSpeed;

    private float timer = 0f;
    public float waitTime = 3f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.transform.LookAt(target);
            BulletStats bulletStats = bullet.GetComponent<BulletStats>();
            bulletStats.Speed = bulletSpeed;
            timer = 0f;
        }
    }
}
