using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : ProjectileController
{
    protected bool isFiring;
    private int numTarget;
    //Reset when get from pool
    protected override void OnEnable()
    {
        isFiring = false;
        numTarget = config.maxTargetHits;
        CancelInvoke();
    }
    protected override void Update()
    {
        base.Update();
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