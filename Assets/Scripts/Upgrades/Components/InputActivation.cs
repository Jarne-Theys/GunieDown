using UnityEngine.InputSystem;
using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class InputActivationComponent : UpgradeComponentBase
{
    [SerializeField]
    public InputActionReference inputAction;
    private GameObject targetPlayer;
    private List<IUpgradeComponent> activeRuntimeComponents;

    private Action<GameObject, List<IUpgradeComponent>> _onActivate; // Modified delegate type

    public Action<GameObject, List<IUpgradeComponent>> onActivate // Modified delegate type
    {
        get => _onActivate;
        set => _onActivate = value;
    }

    // Parameterless constructor required for Activator.CreateInstance
    public InputActivationComponent() {}

    public InputActivationComponent(Action<GameObject, List<IUpgradeComponent>> onActivate)
    {
        this.onActivate = onActivate;
    }

    public void SetRuntimeComponents(List<IUpgradeComponent> components)
    {
        activeRuntimeComponents = components;
    }

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
        _onActivate?.Invoke(targetPlayer, activeRuntimeComponents); // Pass runtimeComponents
    }

    public override void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        // Manually trigger the activation event
        _onActivate?.Invoke(player, runtimeComponents); // Pass runtimeComponents
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