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
        foreach (var item in Resources.LoadAll<ProjectileConfig>("/ScritableObjects/Projectile")) 
        {

            if (!dictProjectile.ContainsKey(item.projectileType))
            {
                listProjectile.Add(item);
                dictProjectile[item.projectileType] = item;
            }
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