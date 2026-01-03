using System.Collections.Generic;
using UnityEngine;

public class LaserController : ProjectileController
{
    private int hitsLeft;
    private HashSet<int> hitSet = new HashSet<int>();

    protected override void OnEnable()
    {
        base.OnEnable();
        hitSet.Clear();
        hitsLeft = Mathf.Max(1, config.maxTargetHits);
    }

    protected override void OnFire()
    {
        transform.position = startPosition;
        transform.up = direction.normalized;

        if (rb != null) rb.velocity = direction.normalized * config.speed;
        if (collide != null) { collide.enabled = true; collide.isTrigger = true; }
    }

    protected override void OnHit(Collider2D other)
    {
        int id = other.GetInstanceID();
        if (hitSet.Contains(id)) return;

        hitSet.Add(id);
        TryDamage(other, config.damage);

        if (config.canPierce)
        {
            hitsLeft--;
            if (hitsLeft <= 0) ReturnToPool();
        }
        else ReturnToPool();
    }
}