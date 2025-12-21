using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [Header("Default Pool Size")]
    [SerializeField] private int defaultPoolSize = 20;

    [Header("Optional Parent")]
    [SerializeField] private Transform poolRoot;

    private Dictionary<ProjectileType, ObjectPool> projectilePools = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (poolRoot == null) poolRoot = transform;

        InitProjectilePools();
    }

    private void InitProjectilePools()
    {
        var db = ProjectileDatabase.Instance;
        if (db == null) { Debug.LogError("ProjectileDatabase not found!"); return; }

        foreach (var cfg in db.GetAll())
        {
            if (cfg == null || cfg.projectilePrefab == null) continue;
            CreateProjectilePool(cfg.projectileType, cfg.projectilePrefab, defaultPoolSize);
        }
    }

    private void CreateProjectilePool(ProjectileType type, GameObject prefab, int size)
    {
        if (projectilePools.ContainsKey(type)) return;

        var holder = new GameObject($"Pool_{type}");
        holder.transform.SetParent(poolRoot);

        var pool = holder.AddComponent<ObjectPool>();
        pool.Initialize(prefab, size, holder.transform);

        projectilePools[type] = pool;
    }

    public GameObject GetProjectile(ProjectileType type)
    {
        if (!projectilePools.TryGetValue(type, out var pool))
        {
            // fallback: tạo pool runtime nếu thiếu
            var cfg = ProjectileDatabase.Instance.GetProjectileConfig(type);
            if (cfg == null || cfg.projectilePrefab == null)
            {
                Debug.LogError($"No projectile config/prefab for {type}");
                return null;
            }
            CreateProjectilePool(type, cfg.projectilePrefab, defaultPoolSize);
            pool = projectilePools[type];
        }
        return pool.GetObject();
    }

    public void ReturnProjectile(ProjectileType type, GameObject obj)
    {
        if (obj == null) return;

        if (projectilePools.TryGetValue(type, out var pool))
            pool.ReturnToPool(obj);
        else
            obj.SetActive(false);
    }
}
