using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileDatabase : MonoBehaviour
{
    public ProjectileDatabase Instance;
    [SerializeField] private List<ProjectileConfig> listProjectile;
    Dictionary<ProjectileType, ProjectileConfig> dictProjectile;
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
            if (!dictProjectile.ContainsKey(config.projectileType))
            {
                dictProjectile.Add(config.projectileType, config);
            }
        }
    }
    private ProjectileConfig GetProjectileConfig(ProjectileType projectileType)
    {
        if (dictProjectile.ContainsKey(projectileType))
        {
            return dictProjectile[projectileType];
        }
            return null;
        
    }
}
