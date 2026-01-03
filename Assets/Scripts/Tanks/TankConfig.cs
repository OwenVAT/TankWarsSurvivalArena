using UnityEngine;

[CreateAssetMenu(fileName = "TankConfig", menuName = "Tank/TankConfig", order = 0)]
public class TankConfig : ScriptableObject
{
    [Header("Basic Info")]
    public TankType tankType;
    public string tankName;

    [Header("Stats")]
    public float maxHP = 100f;
    public float armor = 1f;
    public float moveSpeed = 3f;

    [Header("Weapons")]
    [Tooltip("Primary weapon: Shell or Laser")]
    public ProjectileType primaryWeapon = ProjectileType.LightShell;
    public ProjectileType rocketWeapon = ProjectileType.Rocket;

    [Header("Visual")]
    public GameObject tankPrefab;
}



