using TMPro;
using UnityEngine;

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

    [SerializeField] TMP_Text roundText;

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

        humanPlayer.GetComponent<PlayerMovement>().enabled = false;
        humanPlayer.GetComponent<PlayerShoot>().enabled = false;
        //TODO: Disable AI player movement
        //aiPlayer.GetComponent<PlayerMovement>().enabled = false;

        PowerupScreen.SetActive(true);
    }

    public void Continue()
    {
        humanPlayer.transform.position = SpawnPositions.humanPlayerSpawn;
        aiPlayer.transform.position = SpawnPositions.aiPlayerSpawn;

        Powerup healthBoost = new HealthBoost();
        humanPlayerStats.ApplyPowerup(healthBoost);

        roundNumber++;

        ResetStats();

        PowerupScreen.SetActive(false);

        humanPlayer.GetComponent<PlayerMovement>().enabled = true;
        humanPlayer.GetComponent<PlayerShoot>().enabled = true;
        //TODO: Disable AI player movement
        //aiPlayer.GetComponent<PlayerMovement>().enabled = true;
    }
}
