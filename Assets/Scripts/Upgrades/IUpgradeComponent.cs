using UnityEngine;

public interface IUpgradeComponent
{
    // The Activate method can be called when the ability should trigger.
    void Activate(GameObject player);
    void ApplyPassive(GameObject player);
}
