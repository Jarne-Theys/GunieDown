using UnityEngine.InputSystem;
using System;
using UnityEngine;

[Serializable]
public class InputActivationComponent : UpgradeComponentBase
{
    [SerializeField]
    private InputActionReference inputAction;
    private Action<GameObject> onActivate;
    private GameObject targetPlayer;

    public InputActivationComponent(Action<GameObject> onActivate)
    {
        this.onActivate = onActivate;
    }
    
    public void DisableInput()
    {
        inputAction.action.Disable();
    }

    public void EnableInput()
    {
        inputAction.action.Enable();
    }

    private void OnInputPerformed(InputAction.CallbackContext ctx)
    {
        onActivate?.Invoke(targetPlayer);
    }

    public override void Activate(GameObject player)
    {
        // Manually trigger the activation event
        onActivate?.Invoke(player);
        Debug.Log("Manually triggered input component");
    }

    public override void ApplyPassive(GameObject player)
    {
        targetPlayer = player;
        inputAction.action.performed += OnInputPerformed;
    }

    // Ensure unsubscription when disposing of this component
    ~InputActivationComponent()
    {
        inputAction.action.performed -= OnInputPerformed;
    }
}