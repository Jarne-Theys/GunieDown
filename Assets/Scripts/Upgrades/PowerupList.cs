using System.Collections.Generic;
using UnityEngine;

public class PowerupList : MonoBehaviour
{
    public static PowerupList Instance { get; private set; }

    // Assign all available powerups in the inspector.
    [SerializeField]
    private List<Powerup> allPowerups = new List<Powerup>();

    public List<Powerup> AllPowerups => allPowerups;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Optional: persist across scenes
            // DontDestroyOnLoad(gameObject);
        }
    }
}