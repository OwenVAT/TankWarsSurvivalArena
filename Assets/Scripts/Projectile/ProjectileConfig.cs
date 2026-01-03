using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Weapons/Projectile", order = 0)]
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
    public float cooldown = 0.2f;

    [Header("Properties")]
    public float maxDistance = 10f;

    [Header("Laser")]
    public bool canPierce = false;
    public int maxTargetHits = 1;

    [Header("Rocket / AOE")]
    public bool isAOE = false;
    public float aoeRadius = 2f;
    public bool damageSelf = false;

    [Header("VFX")]
    public GameObject hitEffectPrefab;
    public GameObject explosionEffectPrefab;
}
