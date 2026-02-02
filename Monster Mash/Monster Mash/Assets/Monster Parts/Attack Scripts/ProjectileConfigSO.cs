using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "MonsterPartNameProjectileConfig", menuName = "ScriptableObjects/Attacks/ProjectileConfigSO")]
public class ProjectileConfigSO : ScriptableObject
{
    [SerializeField] protected NewProjectile projectilePrefab;
    [SerializeField] protected float projectileSpeed = 20;
    [SerializeField] protected float projectileLifetime = 10;

    [Header("Object Pool Settings")]
    [SerializeField] protected int poolStartSize = 10;
    [SerializeField] protected int poolMaxSize = 100;
    [SerializeField] protected bool preWarm = true;

    public IObjectPool<NewProjectile> ObjectPool { get; private set; }

    protected int damage;
    protected playerController playerRef;
    private Transform projectileMuzzle;
    private NewMonsterPart partRef;
    

    public void SetupPool(ProjectileRefrenceData data)
    {
        damage = data.Damage;
        playerRef = data.PlayerRef;
        partRef = data.MonsterPartRef;
        projectileMuzzle = data.ProjectileMuzzle;

        // ?? is compound assignment syntax which checks if the refrence is null before assigning it.
        // OnAwakenTheBeast might be invoked multiple times and we dont want to waste resources overwriting the pool each time
        ObjectPool ??= new ObjectPool<NewProjectile>(CreateProjectile,
            OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, false, poolStartSize, poolMaxSize);

        if (preWarm)
        {
            PreWarm();
        }
    }

    private void PreWarm()
    {
        // player starts with some projectiles already in the pool.
        // This helps with performance by creating all the projectiles when the player spawns in instead of when the first few projectiles are fired during gameplay.
        List<NewProjectile> projectiles = new();
       
        for (int i = 0; i < poolStartSize; i++)
        {
            projectiles.Add(ObjectPool.Get());
        }

        foreach (var projectile in projectiles)
        {
            ObjectPool.Release(projectile);
        }
    }

    protected virtual NewProjectile CreateProjectile()
    {
        NewProjectile projectileInstance = Instantiate(projectilePrefab);
        SetupProjectile(projectileInstance);

        return projectileInstance;
    }

    protected virtual void SetupProjectile(NewProjectile projectileInstance)
    {
        projectileInstance.ObjectPool = ObjectPool;
        projectileInstance.Speed = projectileSpeed;
        projectileInstance.Damage = damage;
        projectileInstance.LifeTime = projectileLifetime;
        projectileInstance.PlayerRef = playerRef;
    }

    protected virtual void OnGetFromPool(NewProjectile pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
        pooledObject.transform.SetPositionAndRotation(projectileMuzzle.transform.position, CalculateSpawnRotation(pooledObject));
    }

    private Quaternion CalculateSpawnRotation(NewProjectile pooledObject)
    {
        // There were too many issues with using the muzzle rotation because of the rotations inherited from the bone.
        // So instead I just set the rotation manualy based on the input direction

        Quaternion rotation = Quaternion.identity;
        var capsule = pooledObject.ColiiderRef as CapsuleCollider;

        Vector3 forward = new (0, 90, 0);
        Vector3 backward = new (0, -90, 0);
        Vector3 up = new (-90, 0, 0);
        Vector3 down = new (90, 0, 0);

        switch (playerRef.LastInputDirection)
        {
            case playerController.InputDirection.Forward:
                rotation = Quaternion.Euler(forward);
                capsule.direction = 0;
                break;
            case playerController.InputDirection.Backward:
                rotation = Quaternion.Euler(backward);
                capsule.direction = 0;
                break;
            case playerController.InputDirection.Up:
                rotation = Quaternion.Euler(up);
                capsule.direction = 1;
                break;
            case playerController.InputDirection.Down:
                rotation = Quaternion.Euler(down);
                capsule.direction = 1;
                break;
        }

        return rotation;
    }

    protected virtual void OnReleaseToPool(NewProjectile pooledObject)
    {
        pooledObject.IsReleased = false;
        pooledObject.gameObject.SetActive(false);
    }

    protected virtual void OnDestroyPooledObject(NewProjectile pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
}
