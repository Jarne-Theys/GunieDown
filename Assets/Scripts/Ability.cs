using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Ability : Upgrade
{
    public InputAction abilityAction {get; protected set;}
}

class Dash : Ability
{
    public Dash()
    {
        Name = "Dash";
        Description = "Dashes in the direction you are moving";
        abilityAction = InputSystem.actions.FindAction("Dash");
    }
}