using UnityEngine.InputSystem;
using System;
using UnityEngine;

public class InputActivationComponent : IAbilityComponent
{
    private InputAction inputAction;
    private Action<GameObject> onActivate;

    // Constructor receives an InputAction (configured externally) and a callback
    public InputActivationComponent(InputAction inputAction, Action<GameObject> onActivate)
    {
        this.inputAction = inputAction;
        this.onActivate = onActivate;
        // Enable and subscribe to the input action
        inputAction.Enable();
        inputAction.performed += ctx => onActivate?.Invoke(null);
    }

    public void Activate(GameObject player)
    {
        // This method can be used if you want to trigger the effect programmatically.
        //onActivate?.Invoke(player);
        Debug.Log("Manually triggered input component");

    }
}
