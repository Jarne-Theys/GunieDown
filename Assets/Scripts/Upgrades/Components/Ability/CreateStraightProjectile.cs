using System;
using UnityEngine;

[Serializable]
public class CreateStraightProjectile : UpgradeComponentBase
{
    [SerializeField]
    private GameObject projectilePrefab;
    
    [SerializeField]
    private float projectileSpeed = 0f;
    
    [SerializeField]
    private float projectileDamage = 0;
    
    [SerializeField]
    private float projectileLifeTime = 1f;
    
    public CreateStraightProjectile() {}
    
    public override void Activate(GameObject player)
    {

    }
}
