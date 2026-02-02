using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterPartNameSuctionProjectileConfig", menuName = "ScriptableObjects/Attacks/SuctionProjectileConfigSO")]
public class SuctionProjectileConfigSO : ProjectileConfigSO
{
    [Header("Suction Settings")]
    [SerializeField] private float totalTickTime = 5f;

    protected override void SetupProjectile(NewProjectile projectileInstance)
    {
        base.SetupProjectile(projectileInstance);
        SuctionProjectile suctionProjectile = projectileInstance as SuctionProjectile;
        suctionProjectile.TotalTickTime = totalTickTime;
    }
}
