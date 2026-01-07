using UnityEngine;

[RequireComponent(typeof(TankController))]
public class TankWeaponSystem : MonoBehaviour
{
    [SerializeField] private TankController tank;

    [Header("Rocket Bezier")]
    [SerializeField] private float curveFactor = 0.25f;
    [SerializeField] private float startStraightLen = 1.2f;
    [SerializeField] private float endStraightLen = 0.8f;
    [SerializeField] private float verticalCurvePow = 2.5f;
    [SerializeField] private int trajectorySegments = 25;

    private float lastPrimaryFireTime = float.MinValue;
    private float lastRocketFireTime = float.MinValue;

    private void Awake()
    {
        if (tank == null) tank = GetComponent<TankController>();
    }

    public bool CanFirePrimary()
    {
        ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(tank.GetPrimaryWeapon());
        if (cfg == null) return false;
        return Time.time >= (lastPrimaryFireTime + cfg.cooldown);
    }

    public bool CanFireRocket()
    {
        ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(tank.GetRocketWeapon());
        if (cfg == null) return false;
        return Time.time >= (lastRocketFireTime + cfg.cooldown);
    }

    public void FirePrimary(Vector2 fireDirection)
    {
        ProjectileType type = tank.GetPrimaryWeapon();
        ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(type);
        if (!CanFirePrimary()) return;

        InternalFire(type, cfg, fireDirection.normalized, 1f, null, null, null);
        lastPrimaryFireTime = Time.time;
    }

    // Rocket theo Bezier
    public void FireRocket(Vector2 aimDirection, float power01)
    {
        ProjectileType type = tank.GetRocketWeapon();
        ProjectileConfig cfg = ProjectileDatabase.Instance.GetProjectileConfig(type);
        if (!CanFireRocket()) return;

        float power = Mathf.Clamp01(power01);
        if (power <= 0.05f) return;

        Vector2 firePointDirection = (aimDirection.sqrMagnitude > 0.0001f) ? aimDirection.normalized : tank.firePoint.up;

        Vector2 start = tank.firePoint.position;

        // Max range theo config + power
        float maxRange = cfg.maxDistance * power;

        
        float speed = cfg.speed * power;

        // Side ổn định để preview và fire khớp nhau:
        // Ví dụ: aim.x >= 0 => cong 1 phía, aim.x < 0 => cong phía còn lại
        int side = (firePointDirection.x >= 0f) ? 1 : -1;




        Utilities.RocketBezierPath path = Utilities.BuildRocketBezier(
            start,
            firePointDirection,
            maxRange,
            speed,
            curveFactor,
            startStraightLen,
            endStraightLen,
            verticalCurvePow,
            side
        );

        InternalFire(type, cfg, firePointDirection, power, null, null, path);
        lastRocketFireTime = Time.time;
    }

    private void InternalFire(
        ProjectileType type,
        ProjectileConfig cfg,
        Vector2 dir,
        float power01,
        Vector2? rocketV0,
        Vector2? rocketAcc,
        Utilities.RocketBezierPath? rocketBezier
    )
    {
        GameObject obj = PoolManager.Instance.GetProjectile(type);
        obj.transform.position = tank.firePoint.position;
        obj.transform.rotation = tank.firePoint.rotation;

        ProjectileController proj = obj.GetComponent<ProjectileController>();

        // Rocket -> FireBezier nếu có path + đúng controller
        if (rocketBezier.HasValue && proj is RocketController rocket)
        {
            var path = rocketBezier.Value;
            rocket.FireBezier(
                path.p0, path.p1, path.p2, path.p3,
                tank.gameObject.layer,
                cfg.hitMask,
                cfg.lifeTime, // vẫn dùng lifeTime làm "timeout an toàn"
                (go) => PoolManager.Instance.ReturnProjectile(type, go),
                path.flightTime
            );
            return;
        }

        // Các projectile khác giữ nguyên
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
