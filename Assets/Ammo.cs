using TMPro;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    TMP_Text text;

    [SerializeField]
    WeaponStats humanPlayerWeaponStats;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.text = humanPlayerWeaponStats.CurrentAmmo + " / " + humanPlayerWeaponStats.MaxAmmo;
    }
}
