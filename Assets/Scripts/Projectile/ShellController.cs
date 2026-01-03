using UnityEngine;

public class ShellController : ProjectileController
{
    protected override void OnFire()
    {
        transform.position = startPosition;
        transform.up = direction.normalized;

        if (rb != null) rb.velocity = direction.normalized * config.speed;
        if (collide != null) { collide.enabled = true; collide.isTrigger = true; }
    }

    protected override void OnHit(Collider2D other)
    {
        TryDamage(other, config.damage);
        ReturnToPool();
    }
}