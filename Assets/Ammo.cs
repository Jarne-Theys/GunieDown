using TMPro;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [Tooltip("Which UpgradeDefinition should drive *this* panel?")]
    [SerializeField] private UpgradeDefinition targetDefinition;

    [Tooltip("Drag your UpgradeManager here")]
    [SerializeField] private UpgradeManager upgradeManager;

    private TMP_Text text;
    private ProjectileComponentBase weapon;  

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        upgradeManager.OnUpgradeAcquired += HandleUpgradeAcquired;
    }

    void OnDisable()
    {
        upgradeManager.OnUpgradeAcquired -= HandleUpgradeAcquired;
        UnsubscribeFromCurrentWeapon();
    }

    private void HandleUpgradeAcquired(UpgradeDefinition upgradeDefinition, IUpgradeComponent upgradeComponentInterface)
    {
        // only care about *my* definition
        if (upgradeDefinition != targetDefinition) 
            return;

        // if it happens to be a projectile component, hook in:
        if (upgradeComponentInterface is ProjectileComponentBase projectileComponentBase)
        {
            UnsubscribeFromCurrentWeapon();
            weapon = projectileComponentBase;

            // immediately sync text
            UpdateText(projectileComponentBase.CurrentAmmo, projectileComponentBase.MaxAmmo);

            // and subscribe to realtime changes
            weapon.OnAmmoChanged += UpdateText;

            // make sure the panel is visible
            gameObject.SetActive(true);
        }
    }

    private void UnsubscribeFromCurrentWeapon()
    {
        if (weapon != null)
            weapon.OnAmmoChanged -= UpdateText;

        weapon = null;
        // optionally hide until reacquired
        gameObject.SetActive(false);
    }

    private void UpdateText(int current, int max)
    {
        text.text = $"{current} / {max}";
    }
}
