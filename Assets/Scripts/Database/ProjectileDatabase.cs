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
        dictProjectile = new Dictionary<ProjectileType, ProjectileConfig>();
        foreach (ProjectileConfig cfg in listProjectile)
        {
            dictProjectile[cfg.projectileType] = cfg;
        }
    }

    public ProjectileConfig GetProjectileConfig(ProjectileType type)
    {
        dictProjectile.TryGetValue(type, out ProjectileConfig cfg);
        return cfg;
    }
        

    public List<ProjectileConfig> GetListProjectile()
    {
        return listProjectile;
    }
}