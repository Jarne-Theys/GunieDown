using UnityEngine;

public abstract class UpgradeComponentBase : IUpgradeComponent
{
    //public string componentName;

    protected UpgradeComponentBase()
    {
        //componentName = this.GetType().Name;
    }

    public virtual void Activate(GameObject player) {}
    public virtual void ApplyPassive(GameObject player) {}
}