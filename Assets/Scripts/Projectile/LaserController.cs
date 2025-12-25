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
        hitsLeft = config.maxTargetHits;
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
        int id = other.GetInstanceID();
        if (hitSet.Contains(id)) return;
        hitSet.Add(id);

        TryDamage(other, config.damage);

        if (config.canPierce)
        {
            hitsLeft--;
            if (hitsLeft <= 0) 
                ReturnToPool();
        }
        else
        {
            ReturnToPool();
        }
    }
}
