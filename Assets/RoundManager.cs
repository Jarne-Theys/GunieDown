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
        roundNumber++;
        ResetStats();
    }

    public void aiPlayerScored()
    {
        aiPlayerScore++;
        Debug.Log("Player 2 scored! Player 1: " + humanPlayerScore + " Player 2: " + aiPlayerScore);
        roundNumber++;
        ResetStats();
    }

    private void ResetStats()
    {
        humanPlayerStats.ResetStats();
        aiPlayerStats.ResetStats();
        roundNumber++;
    }

    void Start()
    {
        PowerupScreen.SetActive(true);
        humanPlayerStats = humanPlayer.GetComponent<PlayerStats>();
        aiPlayerStats = aiPlayer.GetComponent<PlayerStats>();

        Debug.Log("Before healthboost: " + humanPlayerStats);

        Powerup healthBoost = new HealthBoost();
        humanPlayerStats.ApplyPowerup(healthBoost);

        Debug.Log("After one healthboost: " + humanPlayerStats);

        humanPlayerStats.ApplyPowerup(healthBoost);

        Debug.Log("After two healthboosts: " + humanPlayerStats);

        ResetStats();

        PowerupScreen.SetActive(false);
    }

    void Update()
    {
        roundText.text = "Round: " + roundNumber + " Score: " + humanPlayerScore + " - " + aiPlayerScore;

        if (aiPlayerStats.Health <= 0 || humanPlayerStats.Health <= 0)
        {
            humanPlayer.GetComponent<PlayerMovement>().enabled = false;
            aiPlayer.GetComponent<PlayerMovement>().enabled = false;
        }

        if (aiPlayerStats.Health <= 0)
        {
            humanPlayerScored();
        }
        else if (humanPlayerStats.Health <= 0)
        {
            aiPlayerScored();
        }
    }
}
