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
    
    /**
     * Virtual is used here to allow for overriding in specific circumstances, such as the projectile components.
     * In general, this method should NOT be overridden, but instead ExecuteActivation should be overridden to change the activate behaviour
     */
    public virtual void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
        Debug.Log($"{GetType().Name}.activationDelay = {activationDelay}");

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
    
    protected virtual void ExecuteActivation(GameObject player, List<IUpgradeComponent> runtimeComponents)
    {
    }
    
    /**
     * This gets ran the instant a powerup is applied, so it should be used both for stat increases (or "passives")
     * as well as any setup needed for the component to work, similar to MonoBehaviour's "Start" method.
     */
    public virtual void ApplyPassive(GameObject player) {}
    
}