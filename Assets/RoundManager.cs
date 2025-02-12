using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public int player1Score;
    public int player2Score;

    int roundNumber = 1;

    public GameObject player1;
    public GameObject player2;

    public void Player1Scored()
    {
        player1Score++;
        Debug.Log("Player 1 scored! Player 1: " + player1Score + " Player 2: " + player2Score);
        roundNumber++;
        ResetStats();
    }

    public void Player2Scored()
    {
        player2Score++;
        Debug.Log("Player 2 scored! Player 1: " + player1Score + " Player 2: " + player2Score);
        roundNumber++;
        ResetStats();
    }

    private void ResetStats()
    {
        player1.GetComponent<PlayerStats>().ResetStats();
        player2.GetComponent<PlayerStats>().ResetStats();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
