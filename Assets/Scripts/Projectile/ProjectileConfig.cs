using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Projectile",menuName ="Weapons/Projectile",order =0)]
public class ProjectileConfig : ScriptableObject
{
    [Header("Info")]
    public ProjectileType projectileType;
    public string projectileName;
    public GameObject projectilePrefab;
    public LayerMask hitMask;

    [Header("Stats")]
    public float damage = 10f;
    public float speed = 10f;
    public float lifeTime = 1f;
    [Tooltip("Time before nextime shoot (seconds)")]public float cooldown = 0.2f;
    

    [Header("Properties")]
    public float maxDistance = 10f;
    public bool curve = false;
    public bool canPierce = false;
    public int maxTargetHits = 1;
    public bool isAOE = false;
    public float aoeRadius = 2f;
    public bool damageSelf = false;


    [Header("Effect")]
    public GameObject hitEffectPrefab;
    public GameObject explosionEffectPrefab;
    


}
public enum ProjectileType
{
    LightShell = 101,
    MediumShell,
    HeavyShell,
    Laser,
    Rocket
}