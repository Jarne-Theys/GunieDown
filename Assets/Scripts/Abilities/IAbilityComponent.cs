using UnityEngine;

public interface IAbilityComponent
{
    // The Activate method can be called when the ability should trigger.
    void Activate(GameObject player);
}
