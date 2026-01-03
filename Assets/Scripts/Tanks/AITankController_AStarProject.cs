using UnityEngine;

[RequireComponent(typeof(TankWeaponSystem))]
[RequireComponent(typeof(AStarFollower2D))]
public class AITankController_AStarProject : TankController
{
    [Header("Refs")]
    public Transform player;

    private TankWeaponSystem weapon;
    private AStarFollower2D follower;

    [Header("Masks")]
    public LayerMask projectileMask;
    public LayerMask pickupMask;

    [Header("Pickup")]
    public float pickupScanRadius = 10f;
    public float lowHPThreshold = 0.55f;

    [Header("Dodge")]
    public float dodgeScanRadius = 6f;
    public float dodgeWeight = 1.5f;
    public float dodgeTimeHorizon = 0.55f;

    [Header("Combat")]
    public float shootRange = 12f;
    public float rocketRange = 14f;
    public float fireAngleTolerance = 10f;

    private Transform currentTarget;
    private float targetUpdateTimer = 0f;
    public float targetUpdateInterval = 0.25f;

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponent<TankWeaponSystem>();
        follower = GetComponent<AStarFollower2D>();
    }

    protected override void Start()
    {
        base.Start();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isDead) return;

        targetUpdateTimer -= Time.deltaTime;
        if (targetUpdateTimer <= 0f)
        {
            targetUpdateTimer = targetUpdateInterval;
            currentTarget = ChooseTarget();
            if (currentTarget != null) follower.SetTarget(currentTarget);
        }

        // Aim ưu tiên player để bắn
        if (player != null)
        {
            Vector2 toPlayer = (player.position - transform.position);
            if (toPlayer.magnitude <= shootRange * 1.2f) SetAimDirection(toPlayer);
        }

        TryCombat();
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;

        Vector2 desired = follower.GetDesiredDirection();
        Vector2 dodge = ComputeProjectileDodge();

        Vector2 finalMove = Vector2.ClampMagnitude(desired + dodge * dodgeWeight, 1f);
        SetMoveInput(finalMove);

        base.FixedUpdate();
    }

    private Transform ChooseTarget()
    {
        float maxHP = baseConfig != null ? baseConfig.maxHP : 100f;
        float hp01 = (maxHP <= 0.01f) ? 0f : currentHP / maxHP;

        if (hp01 <= lowHPThreshold)
        {
            var pu = FindNearestPickup();
            if (pu != null) return pu.transform;
        }

        var near = FindNearestPickup();
        if (near != null && Vector2.Distance(transform.position, near.transform.position) <= 3.5f)
            return near.transform;

        return player;
    }

    private GameObject FindNearestPickup()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, pickupScanRadius, pickupMask);
        GameObject best = null;
        float bestD = float.MaxValue;

        foreach (var h in hits)
        {
            if (h == null) continue;
            float d = Vector2.Distance(transform.position, h.transform.position);
            if (d < bestD) { bestD = d; best = h.gameObject; }
        }
        return best;
    }

    private Vector2 ComputeProjectileDodge()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, dodgeScanRadius, projectileMask);
        Vector2 dodge = Vector2.zero;

        foreach (var h in hits)
        {
            if (h == null) continue;
            Rigidbody2D prb = h.attachedRigidbody;
            if (prb == null) continue;

            Vector2 pPos = h.transform.position;
            Vector2 pVel = prb.velocity;
            if (pVel.sqrMagnitude < 0.5f) continue;

            Vector2 toMe = (Vector2)transform.position - pPos;
            float t = Vector2.Dot(toMe, pVel) / pVel.sqrMagnitude;
            if (t < 0f || t > dodgeTimeHorizon) continue;

            Vector2 closest = pPos + pVel * t;
            float dist = Vector2.Distance(transform.position, closest);

            if (dist < 1.2f)
            {
                Vector2 perp = Vector2.Perpendicular(pVel).normalized;
                float side = Mathf.Sign(Vector2.Dot(perp, (Vector2)transform.position - closest));
                dodge += perp * side * (1.2f - dist);
            }
        }

        return dodge;
    }

    private void TryCombat()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position);
        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        float angle = Vector2.Angle(turretTransform.up, dir);
        bool aimOk = angle <= fireAngleTolerance;

        if (dist <= shootRange && aimOk)
            weapon.FirePrimary(dir);

        if (dist <= rocketRange && weapon.CanFireRocket())
        {
            float power = Mathf.Clamp01(dist / Mathf.Max(1f, rocketRange));
            weapon.FireRocket(dir, power);
        }
    }
}
