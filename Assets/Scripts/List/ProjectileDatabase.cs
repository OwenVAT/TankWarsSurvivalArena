using System.Collections.Generic;
using UnityEngine;

public class ProjectileDatabase : MonoBehaviour
{
    public static ProjectileDatabase Instance;

    [SerializeField] private List<ProjectileConfig> listProjectile;
    private Dictionary<ProjectileType, ProjectileConfig> dictProjectile;

    private void Awake()
    {
        if (Instance != null) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitDictionary();
    }

    private void InitDictionary()
    {
        dictProjectile = new Dictionary<ProjectileType, ProjectileConfig>();
        foreach (ProjectileConfig config in listProjectile)
        {
            if (config == null) 
                continue;
            dictProjectile[config.projectileType] = config;
        }
    }

    public ProjectileConfig GetProjectileConfig(ProjectileType projectileType)
    {
        if (dictProjectile != null && dictProjectile.TryGetValue(projectileType, out ProjectileConfig cfg))
            return cfg;
        return null;
    }

    public List<ProjectileConfig> GetAll() => listProjectile;
}
