using UnityEngine;
using UnityEngine.Pool;

public class ProjectileRefrenceData
{
    public int Damage { get;  set; }
    public playerController PlayerRef {  get;  set; }
    public Transform ProjectileMuzzle { get;  set; }
    public NewMonsterPart MonsterPartRef { get; set; }
}


public class ProjectileNeutral : NeutralAttack
{
    [SerializeField] protected ProjectileConfigSO projectileConfig;
    [SerializeField] protected Transform projectileMuzzle;
    private ProjectileRefrenceData projectileRefrenceData;

    public ProjectileNeutral()
    {
        DamageRange = DamageRange.Range2;
    }

    public override void Init(NewMonsterPart monsterPartRef, MonsterPartVisual monsterPartVisualRef)
    {
        base.Init(monsterPartRef, monsterPartVisualRef);

        projectileRefrenceData = new ProjectileRefrenceData
        {
            Damage = Damage,
            PlayerRef = monsterPartRef.myMainSystem.myPlayer,
            ProjectileMuzzle = projectileMuzzle,
            MonsterPartRef = monsterPartRef
        };

        projectileConfig.SetupPool(projectileRefrenceData);
    }

    public override void TriggerAttackRelease()
    {
        base.TriggerAttackRelease();
        NewProjectile projectile = projectileConfig.ObjectPool.Get();
        projectile.Fire();
    }
}
