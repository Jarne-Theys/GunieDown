using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    private WeaponStats weaponStats;
    private float shootCooldownTimer = 0f;
    private float reloadTimer = 0f;

    private InputAction shootAction;

    [SerializeField]
    TMP_Text debugText;

    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Attack");
        weaponStats = GetComponent<WeaponStats>();
        Debug.Log(weaponStats.ToString());
    }

    void Update()
    {
        // Check if weapon has ammo
        if (weaponStats.CurrentAmmo <= 0)
        {
            debugText.text = "Reloading...";
            // If not, reload
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= weaponStats.ReloadTime)
            {
                weaponStats.CurrentAmmo = weaponStats.MaxAmmo;
                reloadTimer = 0f;
            }
            // We just reloaded, wait for next frame to check fire conditions
            return;
        }

        // Check if our weapon is on cooldown

        // Calculate the time interval between shots in seconds.
        float timeBetweenShots = 60f / weaponStats.FireRate;
        shootCooldownTimer += Time.deltaTime;
        if (shootCooldownTimer < timeBetweenShots)
        {
            debugText.text = "Cooldown...";
            return;
        }

        debugText.text = "Waiting...";

        if (shootAction.IsPressed())
        {
            debugText.text = "Firing...";
            Shoot();
            weaponStats.CurrentAmmo--;
            shootCooldownTimer = 0f;
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

        var bulletStats = bullet.GetComponent<ProjectileStats>();

        // Set the bullet stats
        var playerStats = GetComponent<PlayerStats>();

        bulletStats.Damage = playerStats.BulletDamage;
        bulletStats.Speed = playerStats.BulletSpeed;

        // Draw the debug line
        Debug.DrawLine(transform.position, transform.position + shootDirection * 10f, Color.red, 5f);

    }
}
