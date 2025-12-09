using UnityEngine;
using UnityEngine.Pool;

public class ProjectileNeutral : NeutralAttack
{

    [SerializeField] private NewProjectile projectilePrefab;
    [SerializeField] private Transform projectileMuzzle;
    [SerializeField] private float projectileSpeed;

    [Header("Object Pool Settings")]
    [SerializeField] private int poolStartSize;
    [SerializeField] private int poolMaxSize;

    private IObjectPool<NewProjectile> objectPool;

    public ProjectileNeutral()
    {
        DamageRange = DamageRange.Range2;
    }

    public override void Init(NewMonsterPart monsterPartRef, MonsterPartVisual monsterPartVisualRef)
    {
        base.Init(monsterPartRef, monsterPartVisualRef);
        SetupPool();
    }

    public override void TriggerAttackRelease()
    {
        base.TriggerAttackRelease();
        NewProjectile projectile = objectPool.Get();
    }

    public override void PassDamage()
    {
        if (monsterPartVisualRef.neutralHitVFXManager == null) { return; }
        monsterPartVisualRef.neutralHitVFXManager.damage = Damage;
    }

    public override void statusEffectAndDamageCalculations()
    {
        if (monsterPartVisualRef.neutralHitVFXManager == null) { return; }
        monsterPartVisualRef.neutralHitVFXManager.damage = Damage;
        monsterPartVisualRef.neutralHitVFXManager.updateDamageOnSpray();
    }

    public override void SetupVFX()
    {
        if (monsterPartVisualRef.neutralHitVFXHolder != null || monsterPartVisualRef.neutralDefaultSprayVFXHolder != null)
        {
            StoredParentSetup();
        }
    }

    #region Object Pooling

    private void SetupPool()
    {
        // ?? is compound assignment syntax which checks if the refrence is null before assigning it.
        // OnAwakenTheBeast might be invoked multiple times and we dont want to waste resources overwriting the pool each time
        objectPool ??= new ObjectPool<NewProjectile>(CreateProjectile, 
            OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, false, poolStartSize, poolMaxSize);
    }

    private NewProjectile CreateProjectile()
    {
        NewProjectile projectileInstance = Instantiate(projectilePrefab);

        projectileInstance.ObjectPool = objectPool;
        projectileInstance.Speed = projectileSpeed;
        projectileInstance.Damage = Damage;
        projectileInstance.PlayerRef = monsterPartRef.myMainSystem.myPlayer;

        return projectileInstance;
    }

    private void OnGetFromPool(NewProjectile pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
        pooledObject.transform.SetPositionAndRotation(projectileMuzzle.position, projectileMuzzle.rotation);
        pooledObject.Fire();
    }

    private void OnReleaseToPool(NewProjectile pooledObject)
    {
        pooledObject.IsReleased = false;
        pooledObject.gameObject.SetActive(false);
    }

    private void OnDestroyPooledObject(NewProjectile pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
    #endregion
}
