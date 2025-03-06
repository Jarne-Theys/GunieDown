using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TestUpgradeSO", menuName = "Scripts/Test Upgrade SO")]
public class TestUpgradeSO : ScriptableObject
{
    [SerializeReference]
    public List<IUpgradeComponent> components = new List<IUpgradeComponent>();
}