using System;
using UnityEngine;

public abstract class ProjectileController : MonoBehaviour
{
    [SerializeField] protected ProjectileConfig config;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Collider2D collide;

    protected Vector2 startPosition;
    protected Vector2 direction;
    protected Vector2 endPosition;

    protected Vector2 initialVelocity;
    protected Vector2 acceleration;

    protected int ownerLayer;
    protected LayerMask hitMask;

    protected float lifeTimer;
    protected float lifeTime;

    protected bool isInitialized;
    protected bool isReturning;

    protected Action<GameObject> returnToPool;

    protected virtual void OnEnable()
    {
        isInitialized = false;
        isReturning = false;
        lifeTimer = 0f;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
        }
        if (collide != null) collide.enabled = false;
    }

    public void Fire(
        Vector2 startPoint,
        Vector2 dir,
        Vector2 endPoint,
        int shooterLayer,
        LayerMask mask,
        float lifeTime,
        Action<GameObject> returnToPool,
        Vector2 initialVelocity,
        Vector2 acceleration
    )
    {
        this.startPosition = startPoint;
        this.direction = dir;
        this.endPosition = endPoint;
        this.ownerLayer = shooterLayer;
        this.hitMask = mask;
        this.lifeTime = lifeTime;
        this.returnToPool = returnToPool;
        this.initialVelocity = initialVelocity;
        this.acceleration = acceleration;

        isInitialized = true;
        isReturning = false;

        OnFire();
    }

    protected virtual void Update()
    {
        if (!isInitialized || isReturning) return;
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            OnTimeout();
            ReturnToPool();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized || isReturning) return;
        if (other.gameObject.layer == ownerLayer) return;
        if (((1 << other.gameObject.layer) & hitMask.value) == 0) return;
        OnHit(other);
    }

    protected void ReturnToPool()
    {
        if (isReturning) return;
        isReturning = true;
        isInitialized = false;

        if (collide != null) collide.enabled = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (returnToPool != null) returnToPool(gameObject);
        else gameObject.SetActive(false);
    }

    protected bool TryDamage(Collider2D target, float damage)
    {
        IDamagable dam = target.GetComponentInParent<IDamagable>();
        if (dam == null) return false;
        dam.TakeDamage(damage);
        return true;
    }

    protected virtual void OnFire() { }
    protected virtual void OnHit(Collider2D other) { }
    protected virtual void OnTimeout() { }
}