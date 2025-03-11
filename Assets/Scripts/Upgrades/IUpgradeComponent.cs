using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeComponent
{
    void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents) { }
    void ApplyPassive(GameObject player);
}
