using UnityEngine;

public class ForwardLine : MonoBehaviour
{
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.red, 2);
    }
}
