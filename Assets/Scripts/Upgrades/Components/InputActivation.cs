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
    
    private bool isInitialized = false;

    private Action<GameObject, List<IUpgradeComponent>> onActivate; // Modified delegate type

    public Action<GameObject, List<IUpgradeComponent>> OnActivate // Modified delegate type
    {
        get => onActivate;
        set => onActivate = value;
    }

    // Parameterless constructor required for Activator.CreateInstance
    public InputActivationComponent() {}

    public InputActivationComponent(Action<GameObject, List<IUpgradeComponent>> onActivate)
    {
        this.OnActivate = onActivate;
    }

    public void SetRuntimeComponents(List<IUpgradeComponent> components)
    {
        activeRuntimeComponents = components;
        if (targetPlayer != null && onActivate != null) MarkInitialized();
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
        if (!isInitialized)
        {
            Debug.LogWarning("Input received for InputActivationComponent, but it's not initialized. Ignoring.");
            return;
        }

        if (targetPlayer == null || activeRuntimeComponents == null)
        {
            Debug.LogError("OnInputPerformed called, but targetPlayer or activeRuntimeComponents are null. Was initialization correct?");
            return;
        }


        Debug.Log($"Triggering action for {targetPlayer.name} via input {inputAction.action.name}.");
        onActivate?.Invoke(targetPlayer, activeRuntimeComponents);
    }

    protected override void ExecuteActivation(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        onActivate?.Invoke(player, runtimeComponents); 
    }
    
    public void TriggerAction()
    {
        if (!isInitialized)
        {
            Debug.LogError("Attempted to TriggerAction on InputActivationComponent before it was fully initialized!");
            return;
        }

        if (targetPlayer == null || activeRuntimeComponents == null)
        {
            Debug.LogError("TriggerAction called, but targetPlayer or activeRuntimeComponents are null. Was initialization correct?");
            return;
        }

        onActivate?.Invoke(targetPlayer, activeRuntimeComponents);
    }
    
    public void MarkInitialized()
    {
        if (targetPlayer != null && activeRuntimeComponents != null && onActivate != null)
        {
            isInitialized = true;
            EnableInput();
        }
        else
        {
            Debug.LogWarning("InputActivationComponent cannot be marked initialized: targetPlayer, activeRuntimeComponents, or onActivate is null.");
            isInitialized = false;
        }
    }
    
    public override void ApplyPassive(GameObject player)
    {
        targetPlayer = player;
        if (inputAction != null && inputAction.action != null)
            inputAction.action.performed += OnInputPerformed;
        if (activeRuntimeComponents != null && onActivate != null) MarkInitialized();
    }
}