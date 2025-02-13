using TMPro;
using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] PlayerStats PlayerStats;

    void Update()
    {
        text.text = "HP: " + PlayerStats.Health;
    }
}
