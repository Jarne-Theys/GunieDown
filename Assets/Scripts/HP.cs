using TMPro;
using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public void UpdateHP(float newAmount)
    {
        text.text = "HP: " + newAmount;
    }
}
