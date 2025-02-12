using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float shootCooldown = 0.5f;
    private float shootCooldownTimer = 0f;
    private bool canShoot = true;

    private InputAction shootAction;

    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Attack");
    }

    void Update()
    {
        if (shootAction.IsPressed() && canShoot)
        {
            Shoot();
            canShoot = false;
        }
    }

    private void FixedUpdate()
    {
        if (!canShoot)
        {
            shootCooldownTimer += Time.deltaTime;
            if (shootCooldownTimer >= shootCooldown)
            {
                canShoot = true;
                shootCooldownTimer = 0f;
            }
        }
    }

    public void Shoot()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 shootDirection = cameraTransform.forward;

        // Calculate the rotation based on the shoot direction
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection, Vector3.up);

        // Instantiate the bullet with the calculated rotation
        GameObject bullet = Instantiate(bulletPrefab, transform.position + shootDirection * 1f, shootRotation);

        var bulletStats = bullet.GetComponent<BulletStats>();

        // Set the bullet stats
        var playerStats = GetComponent<PlayerStats>();

        bulletStats.Damage = playerStats.BulletDamage;
        bulletStats.Speed = playerStats.BulletSpeed;

        // Draw the debug line
        Debug.DrawLine(transform.position, transform.position + shootDirection * 10f, Color.red, 5f);

    }
}
