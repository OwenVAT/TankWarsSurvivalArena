using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [SerializeField] private int poolSize = 100;
    [SerializeField] private Transform poolRoot;

    private Dictionary<ProjectileType, ObjectPool> projectilePools = new Dictionary<ProjectileType, ObjectPool>();

    private void Awake()
    {
        if (Instance != null) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (poolRoot == null) 
            poolRoot = transform;
        InitProjectilePools();
    }

    private void InitProjectilePools()
    {
        foreach (ProjectileConfig cfg in ProjectileDatabase.Instance.GetListProjectile())
        {
            CreatePool(cfg.projectileType, cfg.projectilePrefab, poolSize);
        }
    }

    private void CreatePool(ProjectileType type, GameObject prefab, int size)
    {
        if (projectilePools.ContainsKey(type)) 
            return;
        GameObject holder = new GameObject($"Pool_{type}");
        holder.transform.SetParent(poolRoot);
        ObjectPool pool = holder.AddComponent<ObjectPool>();
        pool.Initialize(prefab, size, holder.transform);
        projectilePools[type] = pool;
    }

    public GameObject GetProjectile(ProjectileType type)
    {
        if (!projectilePools.TryGetValue(type, out ObjectPool pool))
        {
            ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(type);
            CreatePool(type, cfg.projectilePrefab, poolSize);
            pool = projectilePools[type];
        }
        return pool.GetObject();
    }

    public void ReturnProjectile(ProjectileType type, GameObject obj)
    {
        if (projectilePools.TryGetValue(type, out ObjectPool pool))
            pool.ReturnToPool(obj);
        else
        {
            obj.SetActive(false);
            Destroy(obj);
        }
    }
}