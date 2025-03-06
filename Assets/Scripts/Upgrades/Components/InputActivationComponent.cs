using UnityEngine.InputSystem;
using System;
using UnityEngine;

[Serializable]
public class InputActivationComponent : UpgradeComponentBase
{
    [SerializeField]
    public InputActionReference inputAction;
    private Action<GameObject> _onActivate;
    private GameObject targetPlayer;

    public Action<GameObject> onActivate
    {
        get => _onActivate;
        set => _onActivate = value;
    }

    // Parameterless constructor required for Activator.CreateInstance
    public InputActivationComponent() {}

    public void DisableInput()
    {
        if (inputAction != null && inputAction.action != null)
            inputAction.action.Disable();
    }

    public void EnableInput()
    {
        if (inputAction != null && inputAction.action != null)
            inputAction.action.Enable();
    }

    private void OnInputPerformed(InputAction.CallbackContext ctx)
    {
        _onActivate?.Invoke(targetPlayer); // Use the backing field
    }

    public override void Activate(GameObject player)
    {
        // Manually trigger the activation event
        _onActivate?.Invoke(player); // Use the backing field
        Debug.Log("Manually triggered input component");
    }

    public override void ApplyPassive(GameObject player)
    {
        targetPlayer = player;
        if (inputAction != null && inputAction.action != null)
            inputAction.action.performed += OnInputPerformed;
    }

    ~InputActivationComponent()
    {
        if (inputAction != null && inputAction.action != null)
            inputAction.action.performed -= OnInputPerformed;
    }
}