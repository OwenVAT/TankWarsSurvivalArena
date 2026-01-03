using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

public class RocketController : ProjectileController
{
    [SerializeField] private float knockBackForce = 6f;

    private Vector2 p0, v0, a, Length;
    private float timeRocket;

    protected override void OnFire()
    {
        transform.position = startPosition;

        p0 = startPosition;
        v0 = initialVelocity;
        a = acceleration;
        //base.endPosition= Mathf.Min()

        Vector2 s = p0 + v0*
        sbyte= s0 + v0t + 0.56f * a * timeRocket * timeRocket;


        Length= endPosition - startPosition;
        //Mathf.Cos(Utilities.GetAngle(v0));
        timeRocket = Length.magnitude / (v0.magnitude * Mathf.Cos(Utilities.GetAngle(v0)));

        if (v0.sqrMagnitude > 0.0001f) transform.up = v0.normalized;

        if (rb != null) rb.velocity = Vector2.zero;
        if (collide != null) { collide.enabled = true; collide.isTrigger = true; }
    }

    protected override void Update()
    {
        if (!isInitialized || isReturning) return;

        lifeTimer += Time.deltaTime;

        float t = lifeTimer;
        Vector2 pos = Utilities.GetPosDiagonalShoot(p0, v0, a, t);
        transform.position = pos;

        Vector2 vel = Utilities.GetVelocityDiagonalShoot(v0, a, t);
        if (vel.sqrMagnitude > 0.0001f) 
            transform.up = vel.normalized;
        
        //t = L/(v0*cos(alpha))
        float alpha = Utilities.GetAngle(vel);
        float power = Mathf.Clamp01(GetInputJoystick.Instance.AimInput().magnitude);
        timeRocket = config.maxDistance / (v0.magnitude*Mathf.Cos(alpha));

        // “Chạm đất” top-down: hết lifeTime => rơi xuống đất => nổ
        if (lifeTimer >= Mathf.Min(lifeTime,timeRocket))
        {
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