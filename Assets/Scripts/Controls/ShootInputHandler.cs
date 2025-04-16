using UnityEngine;
using UnityEngine.InputSystem;

public class ShootInputHandler : MonoBehaviour
{
    private InputAction shootAction;
    private ShootHandler shootHandler;

    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Attack");
        shootHandler = GetComponent<ShootHandler>();
        shootAction.performed += shootHandler.Shoot;
    }
}
