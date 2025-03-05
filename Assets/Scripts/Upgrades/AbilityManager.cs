using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public List<Upgrade> Upgrades { get; private set; } = new List<Upgrade>();

    [SerializeField] private InputAction jumpInputAction;
    [SerializeField] private InputAction crouchInputAction;

    private void OnEnable()
    {
        jumpInputAction.Enable();
        crouchInputAction.Enable();
        jumpInputAction.performed += OnJumpInput;
        crouchInputAction.performed += OnCrouchInput;
    }

    private void OnDisable()
    {
        jumpInputAction.performed -= OnJumpInput;
        crouchInputAction.performed -= OnCrouchInput;
        jumpInputAction.Disable();
        crouchInputAction.Disable();
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        foreach (var upgrade in Upgrades)
        {
            if (upgrade.TriggerAction == TriggerAction.Jump)
            {
                upgrade.Activate(gameObject);
            }
        }
    }

    private void OnCrouchInput(InputAction.CallbackContext context)
    {

    }

    public void AddAbility(Ability ability)
    {
        // Apply passive effects immediately.
        ability.ApplyPassiveEffects(gameObject);
        Abilities.Add(ability);
    }
}