using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradeComponentBase : IUpgradeComponent
{
    [SerializeField]
    [Tooltip("Optionally wait x seconds before triggering this component.")]
    private float activationDelay = 0f;

    protected UpgradeComponentBase()
    {
    }

    /// <summary>
    /// Public entry point for activating the component.
    /// Handles the activation delay before calling the specific execution logic.
    /// Derived classes should NOT typically override this. Override ExecuteActivation instead.
    /// </summary>
    public virtual void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        if (activationDelay <= 0f)
        {
            ExecuteActivation(player, runtimeComponents);
            return;
        }
        
        if (CoroutineRunner.Instance != null)
        {
            CoroutineRunner.Instance.StartCoroutine(HandleActivationDelay(player, runtimeComponents));
        }
        else
        {
            Debug.LogError("CoroutineRunner.Instance is null. Cannot run delayed activation.");
        }
    }
    
    private IEnumerator HandleActivationDelay(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        if (activationDelay > 0f)
        {
            yield return new WaitForSeconds(activationDelay);
        }
        ExecuteActivation(player, runtimeComponents);
    }
    
    /// <summary>
    /// Contains the specific activation logic for the derived component.
    /// This method is called by Activate after the activationDelay has passed.
    /// Derived classes should override this method to implement their activation behavior.
    /// </summary>
    protected virtual void ExecuteActivation(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
    }
    
    public virtual void ApplyPassive(GameObject player) {}
    
}