using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="TankStats",menuName ="Tank/TankStats",order = 0)]
public class TankConfig : ScriptableObject
{
    [Header("Basic Info")]
    public TankType tankType;
    public string tankName;

    [Header("Stats")]
    public float maxHP = 100f;
    public float armor = 10f;
    public float moveSpeed = 3f;
  

    [Header("Weapon")]
    public ProjectileType defaultProjectile = ProjectileType.LightShell;
    [Tooltip("Fire rate per second")]
    public int fireRate = 2;

    [Header("Visual")]
    public GameObject TankPrefab;
}

public enum TankType
{
    Tank_Red = 001,
    Tank_Green, 
    Tank_Blue,
    Tank_Orange
}

