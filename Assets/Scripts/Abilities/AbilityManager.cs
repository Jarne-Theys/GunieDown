using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public List<Ability> Abilities { get; private set; } = new List<Ability>();

    // Drag your InputAction (configured to listen for the "shift" key) into this field in the Inspector.
    [SerializeField] private InputAction shiftKey;

    private void OnEnable()
    {
        shiftKey.Enable();
        shiftKey.performed += OnAbilityInput;
    }

    private void OnDisable()
    {
        shiftKey.performed -= OnAbilityInput;
        shiftKey.Disable();
    }

    // This is where the input is checked and the ability activated.
    /**
     * TODO Dynamically trigger abilities instead of all at once.
     * body As title says, we should be able to trigger abilities individually instead of all at once.
     */
    private void OnAbilityInput(InputAction.CallbackContext context)
    {
        // For this example, we'll simply activate all abilities.
        foreach (var ability in Abilities)
        {
            ability.Activate(gameObject);
        }
    }

    // Call this to add abilities at runtime.
    public void AddAbility(Ability ability)
    {
        Abilities.Add(ability);
    }
}
