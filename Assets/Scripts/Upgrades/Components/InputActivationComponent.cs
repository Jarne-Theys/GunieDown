using UnityEngine.InputSystem;
using System;
using UnityEngine;

public class InputActivationComponent : IUpgradeComponent
{
    // Currently unused, as the component manages triggering the action itself, rather than the ability manager (removed)
    //private TriggerAction triggerAction;
    private InputAction inputAction;
    private Action<GameObject> onActivate;
    private GameObject targetPlayer;

    public InputActivationComponent(InputAction inputAction, Action<GameObject> onActivate)
    {
        this.inputAction = inputAction ?? throw new ArgumentNullException(nameof(inputAction));
        this.onActivate = onActivate ?? throw new ArgumentNullException(nameof(onActivate));

        // Enable and subscribe to the input action
        inputAction.Enable();
        inputAction.performed += OnInputPerformed;
        
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void DisableInput()
    {
        inputAction.Disable();
    }

    public void EnableInput()
    {
        inputAction.Enable();
    }

    private void OnInputPerformed(InputAction.CallbackContext ctx)
    {
        onActivate?.Invoke(targetPlayer);
    }

    public void Activate(GameObject player)
    {
        // Manually trigger the activation event
        onActivate?.Invoke(player);
        Debug.Log("Manually triggered input component");
    }

    public void ApplyPassive(GameObject player) { }

    // Ensure unsubscription when disposing of this component
    ~InputActivationComponent()
    {
        inputAction.performed -= OnInputPerformed;
    }
}