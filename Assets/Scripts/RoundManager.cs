using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    List<Powerup> allPowerups = new List<Powerup>
    {
        new HealthBoost(),
        new BulletDamageBoost(),
        new BulletSpeedBoost(),
        //new ArmorBoost()
    };

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
        ShowPowerupSelection();
    }

    public void ShowPowerupSelection()
    {
        humanPlayer.GetComponent<PlayerMovement>().enabled = false;
        humanPlayer.GetComponent<PlayerShoot>().enabled = false;
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
        List<Powerup> chosenPowerups = new List<Powerup>();
        while (chosenPowerups.Count < 3)
        {
            Powerup randomPowerup = allPowerups[Random.Range(0, allPowerups.Count)];
            if (!chosenPowerups.Contains(randomPowerup))
                chosenPowerups.Add(randomPowerup);
        }

        // Create buttons dynamically
        foreach (Powerup powerup in chosenPowerups)
        {
            GameObject buttonObj = Instantiate(PowerupButtonPrefab, PowerupButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = powerup.Name + "\n" + powerup.Description;

            button.onClick.AddListener(() => ApplyPowerupAndContinue(powerup));
        }
    }

    public void ApplyPowerupAndContinue(Powerup selectedPowerup)
    {
        selectedPowerup.Apply(humanPlayer);

        humanPlayer.transform.position = SpawnPositions.humanPlayerSpawn;
        aiPlayer.transform.position = SpawnPositions.aiPlayerSpawnPosition;

        roundNumber++;

        ResetStats();

        PowerupScreen.SetActive(false);

        humanPlayer.GetComponent<PlayerMovement>().enabled = true;
        humanPlayer.GetComponent<PlayerShoot>().enabled = true;
        humanPlayer.GetComponent<PlayerLook>().enabled = true;
    }
}
