using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class TankDatabase : MonoBehaviour
{
    public static TankDatabase Instance;

    [SerializeField] private List<TankConfig> tankConfigList;
    private Dictionary<TankType, TankConfig> tankConfigDict;
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
        tankConfigDict = new Dictionary<TankType, TankConfig>();
        foreach (TankConfig config in tankConfigList)
        {
            if (!tankConfigDict.ContainsKey(config.tankType))
            {
                tankConfigDict.Add(config.tankType, config);
            }
        }
    }
    private TankConfig GetTankConfig(TankType tankType)
    {
        if (tankConfigDict.TryGetValue(tankType, out TankConfig tankConfig)) 
        {
            return tankConfig;
        }
        return null;
    }
}