using UnityEngine;

public abstract class UpgradeComponentBase : IUpgradeComponent
{
    public string componentName;

    protected UpgradeComponentBase()
    {
        componentName = this.GetType().Name;
    }

    public abstract void Activate(GameObject player);
    public abstract void ApplyPassive(GameObject player);
}