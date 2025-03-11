using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradeComponentBase : IUpgradeComponent
{
    //public string componentName;

    protected UpgradeComponentBase()
    {
        //componentName = this.GetType().Name;
    }

    public virtual void Activate(GameObject player, List<IUpgradeComponent> runtimeComponents) { }
    public virtual void ApplyPassive(GameObject player) {}
}