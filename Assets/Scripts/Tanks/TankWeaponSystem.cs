using UnityEngine;

[RequireComponent(typeof(TankController))]
public class TankWeaponSystem : MonoBehaviour
{
    [SerializeField] private TankController tank;
    [SerializeField] private float rocketGravity = 10f; // Vector2.down * g
    
    private float lastPrimaryFireTime = float.MinValue;
    private float lastRocketFireTime = float.MinValue;

    private void Awake()
    {
        if (tank == null) tank = GetComponent<TankController>();
    }

    public bool CanFirePrimary()
    {
        ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(tank.GetPrimaryWeapon());
        if (cfg == null) 
            return false;
        return Time.time >= (lastPrimaryFireTime + cfg.cooldown);
    }

    public bool CanFireRocket()
    {
        ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(tank.GetRocketWeapon());
        if (cfg == null) 
            return false;
        return Time.time >= (lastRocketFireTime + cfg.cooldown);
    }

    public void FirePrimary(Vector2 fireDirection)
    {
        ProjectileType type = tank.GetPrimaryWeapon();
        ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(type);
        if (!CanFirePrimary()) 
            return;

        InternalFire(type, cfg, fireDirection.normalized, 1f, null, null);
        lastPrimaryFireTime = Time.time;
    }

    public void FireRocket(Vector2 aimDirection, float power01)
    {
        ProjectileType type = tank.GetRocketWeapon();
        ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(type);
      
        if (!CanFireRocket()) 
            return;

        float p = Mathf.Clamp01(power01);
        Vector2 dir = aimDirection.sqrMagnitude > 0.0001f ? aimDirection.normalized : (Vector2)tank.firePoint.up;

        Vector2 v0 = dir * cfg.speed * p;
        Vector2 a = Vector2.down * rocketGravity;

        InternalFire(type, cfg, dir, p, v0, a);
        lastRocketFireTime = Time.time;
    }

    private void InternalFire(ProjectileType type, ProjectileConfig cfg, Vector2 dir, float power01, Vector2? rocketV0, Vector2? rocketAcc)
    {
        GameObject obj = PoolManager.Instance.GetProjectile(type);
        obj.transform.position = tank.firePoint.position;
        obj.transform.rotation = tank.firePoint.rotation;

        ProjectileController proj = obj.GetComponent<ProjectileController>();

        Vector2 start = tank.firePoint.position;
        Vector2 end = start + dir * (cfg.maxDistance * power01);

        Vector2 initVel = rocketV0 ?? (dir * cfg.speed);
        Vector2 acc = rocketAcc ?? Vector2.zero;

        proj.Fire(
            start, dir, end,
            tank.gameObject.layer,
            cfg.hitMask,
            cfg.lifeTime,
            (go) => PoolManager.Instance.ReturnProjectile(type, go),
            initVel, acc
        );
    }
}
