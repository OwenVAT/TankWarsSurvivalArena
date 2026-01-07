using System;
using UnityEngine;

public class RocketController : ProjectileController
{
    [SerializeField] private float knockBackForce = 6f;

    private Vector2 p0, p1, p2, p3;
    private float flightTime;     // thời gian bay theo curve (để tới p3)
    private float t01;            // tham số 0..1 trên Bezier

    // NEW API: bắn rocket theo Bezier
    public void FireBezier(
        Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3,
        int shooterLayer,
        LayerMask mask,
        float lifeTime,
        Action<GameObject> returnToPool,
        float flightTime
    )
    {
        // reuse base Fire để set common fields + enable collider + life timer, etc.
        // Nhưng base Fire yêu cầu start/dir/end/v0/a... ta không dùng.
        // => Ta set trực tiếp các field cần và gọi OnFireBezier.

        this.startPosition = p0;
        this.endPosition = p3;
        this.ownerLayer = shooterLayer;
        this.hitMask = mask;
        this.lifeTime = lifeTime;
        this.returnToPool = returnToPool;

        this.p0 = p0; this.p1 = p1; this.p2 = p2; this.p3 = p3;
        this.flightTime = Mathf.Max(0.05f, flightTime);

        isInitialized = true;
        isReturning = false;

        OnFireBezier();
    }

    private void OnFireBezier()
    {
        transform.position = p0;

        // hướng ban đầu theo tiếp tuyến
        Vector2 vel0 = Utilities.BezierCubicDerivative(p0, p1, p2, p3, 0f);
        if (vel0.sqrMagnitude > 0.0001f) transform.up = vel0.normalized;

        if (rb != null) rb.velocity = Vector2.zero;
        if (collide != null) { collide.enabled = true; collide.isTrigger = true; }

        lifeTimer = 0f;
        t01 = 0f;
    }

    protected override void Update()
    {
        if (!isInitialized || isReturning) return;

        // timeout an toàn (tránh kẹt)
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Explode();
            ReturnToPool();
            return;
        }

        // tham số curve theo flightTime để tới đích đúng lúc
        t01 += Time.deltaTime / flightTime;
        float t = Mathf.Clamp01(t01);

        Vector2 pos = Utilities.BezierCubic(p0, p1, p2, p3, t);
        transform.position = pos;

        Vector2 vel = Utilities.BezierCubicDerivative(p0, p1, p2, p3, t);
        if (vel.sqrMagnitude > 0.0001f) transform.up = vel.normalized;

        // “Chạm đất” top-down = tới cuối curve (p3)
        if (t >= 1f)
        {
            transform.position = p3; // snap
            Explode();
            ReturnToPool();
        }
    }

    protected override void OnHit(Collider2D other)
    {
        Explode();
        ReturnToPool();
    }

    private void Explode()
    {
        if (!config.isAOE) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, config.aoeRadius, hitMask);
        foreach (Collider2D h in hits)
        {
            if (h == null) continue;
            if (h.gameObject.layer == ownerLayer && !config.damageSelf) continue;

            TryDamage(h, config.damage);

            IKnockBackable kb = h.GetComponentInParent<IKnockBackable>();
            if (kb != null)
            {
                Vector2 dir = ((Vector2)h.transform.position - (Vector2)transform.position);
                if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
                kb.ApplyKnockBack(dir.normalized, knockBackForce);
            }
        }

        if (config.explosionEffectPrefab != null)
            Instantiate(config.explosionEffectPrefab, transform.position, Quaternion.identity);
    }
}
