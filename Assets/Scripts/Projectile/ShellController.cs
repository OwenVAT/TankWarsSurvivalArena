using UnityEngine;

public class ShellController : ProjectileController
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnFire()
    {
        lifeTime = config.lifeTime;

        ResetRigidbody(rb);
        rb.velocity = direction.normalized * config.speed;

        collide.enabled = true;
        collide.isTrigger = true;

        transform.up = direction.normalized;
    }

    protected override void OnHit(Collider2D other)
    {
        TryDamage(other, config.damage);
        ReturnToPool();
    }
}
