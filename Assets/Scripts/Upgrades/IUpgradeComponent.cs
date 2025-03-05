using UnityEngine;

public interface IUpgradeComponent
{
    void Activate(GameObject player);
    void ApplyPassive(GameObject player);
}
