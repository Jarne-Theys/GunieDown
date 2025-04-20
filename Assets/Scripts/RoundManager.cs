using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public int humanPlayerScore;
    public int aiPlayerScore;

    int roundNumber = 1;

    public GameObject humanPlayer;
    public GameObject aiPlayer;

    private PlayerStats humanPlayerStats;
    private PlayerStats aiPlayerStats;

    [SerializeField] GameObject PowerupScreen;
    public GameObject PowerupButtonPrefab;
    public Transform PowerupButtonContainer;

    [SerializeField] TMP_Text roundText;
    
    [SerializeField]
    private UpgradesList upgradesList;

    [SerializeField]
    private InputActionReference cheatButton;

    [SerializeField]
    private UpgradeManager upgradeManagerHuman;

    [SerializeField] private UpgradeManager upgradeManagerAi;

    [SerializeField]
    private UpgradeDefinition baseWeaponHuman;
    
    [SerializeField]
    private UpgradeDefinition baseWeaponAi;

    public void humanPlayerScored()
    {
        humanPlayerScore++;
        Debug.Log("Player 1 scored! Player 1: " + humanPlayerScore + " Player 2: " + aiPlayerScore);
    }

    public void aiPlayerScored()
    {
        aiPlayerScore++;
        Debug.Log("Player 2 scored! Player 1: " + humanPlayerScore + " Player 2: " + aiPlayerScore);
    }

    private void ResetStats()
    {
        humanPlayerStats.ResetStats();
        aiPlayerStats.ResetStats();
    }

    void Start()
    {
        humanPlayerStats = humanPlayer.GetComponent<PlayerStats>();
        aiPlayerStats = aiPlayer.GetComponent<PlayerStats>();
        PowerupScreen.SetActive(false);
        
        // Previously in OnEnable
        cheatButton.action.performed += ApplyAllUpgrades;
        upgradeManagerHuman.AcquireUpgrade(baseWeaponHuman);
        upgradeManagerAi.AcquireUpgrade(baseWeaponAi);
    }



    void OnDisable()
    {
        if (cheatButton != null && cheatButton.action != null)
        {
            cheatButton.action.performed -= ApplyAllUpgrades;
        }
    }

    private void ApplyAllUpgrades(InputAction.CallbackContext ctx)
    {
        //ResetStats();
        Debug.Log("Pressed cheat button");
        UpgradeDefinition[] allUpgrades = upgradesList.upgrades;
        foreach (UpgradeDefinition upgrade in allUpgrades)
        {
            upgradeManagerHuman.AcquireUpgrade(upgrade);
            upgradeManagerAi.AcquireUpgrade(upgrade);
        }
    }

    void Update()
    {

        roundText.text = "Round: " + roundNumber + " Score: " + humanPlayerScore + " - " + aiPlayerScore;

        if (aiPlayerStats.Health <= 0)
        {
            humanPlayerScored();
        }
        else if (humanPlayerStats.Health <= 0)
        {
            aiPlayerScored();
        }
        else {
            return;
        }

        ResetStats();
        // TODO: uncomment when not training AI
        // ShowPowerupSelection();
    }

    public void ShowPowerupSelection()
    {
        humanPlayer.GetComponent<PlayerMovement>().enabled = false;
        humanPlayer.GetComponent<PlayerLook>().enabled = false;
        //TODO: Disable AI player movement
        //aiPlayer.GetComponent<PlayerMovement>().enabled = false;

        PowerupScreen.SetActive(true);

        // Clear previous buttons
        foreach (Transform child in PowerupButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Select 2-3 random power-ups
        List<UpgradeDefinition> randomUpgrades = new List<UpgradeDefinition>();
        while (randomUpgrades.Count < 3)
        {
            UpgradeDefinition upgrade = upgradesList.GetRandomUpgrade();
            if (!randomUpgrades.Contains(upgrade))
                randomUpgrades.Add(upgrade);
        }

        // Create buttons dynamically
        foreach (UpgradeDefinition upgrade in randomUpgrades)
        {
            GameObject buttonObj = Instantiate(PowerupButtonPrefab, PowerupButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = upgrade.upgradeName + "\n" + upgrade.description;

            button.onClick.AddListener(() => ApplyPowerupAndContinue(upgrade));
        }
    }

    public void ApplyPowerupAndContinue(UpgradeDefinition selectedUpgrade)
    {
        selectedUpgrade.ApplyUpgrade(humanPlayer);

        humanPlayer.transform.position = SpawnPositions.humanPlayerSpawn;
        aiPlayer.transform.position = SpawnPositions.aiPlayerSpawnPosition;

        roundNumber++;

        ResetStats();

        PowerupScreen.SetActive(false);

        humanPlayer.GetComponent<PlayerMovement>().enabled = true;
        humanPlayer.GetComponent<PlayerLook>().enabled = true;
    }
}
