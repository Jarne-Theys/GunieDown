using TMPro;
using UnityEngine;

public class HP : MonoBehaviour
{
    TMP_Text text;

    [SerializeField]
    PlayerStats humanPlayerStats;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.text = "HP: " + humanPlayerStats.Health;
    }
}
