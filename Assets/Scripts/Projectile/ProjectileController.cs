using System;
using UnityEngine;

public abstract class ProjectileController : MonoBehaviour
{
    [SerializeField] protected ProjectileConfig config;

    [Header("Component")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Collider2D collide;
    [SerializeField] protected LineRenderer lineRenderer;

    protected Vector2 startPosition;
    protected Vector2 endPosition;
    protected Vector2 direction;

    protected int ownerLayer;
    protected LayerMask hitMask;

    Action<GameObject> returnToPool;

    protected float lifeTimer;
    protected float lifeTime;
    protected float distanceFly;

    protected bool isInitialized;
    protected bool isReturning;

    protected virtual void OnEnable()
    {
        CancelInvoke();
        isReturning = false;
        isInitialized = false;

        lifeTimer = 0f;
        distanceFly = 0f;

        if (rb != null) ResetRigidbody(rb);
        if (lineRenderer != null) HideLineRenderer(lineRenderer);

        if (collide != null)
        {
            collide.enabled = false;
        }
    }

    public void Fire(Vector2 startPoint, Vector2 direction, Vector2 endPoint, int shooterLayer, LayerMask mask, float lifeTime, Action<GameObject> returnToPool)
    {
        this.startPosition = startPoint;
        this.direction = direction;
        this.endPosition = endPoint;
        this.ownerLayer = shooterLayer;
        this.hitMask = mask;
        this.lifeTime = lifeTime;
        this.returnToPool = returnToPool;

        isInitialized = true;
        isReturning = false;

        OnFire();
    }

    protected virtual void Update()
    {
        if (!isInitialized || isReturning) return;

        lifeTimer += Time.deltaTime;
        distanceFly += config.speed * Time.deltaTime;

        if (lifeTimer >= lifeTime || distanceFly >= config.maxDistance)
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

        CancelInvoke();
        OnBeforeReturn();

        if (returnToPool != null) returnToPool(gameObject);
        else gameObject.SetActive(false);
    }

    protected virtual void OnBeforeReturn()
    {
        if (rb != null) ResetRigidbody(rb);
        if (collide != null) collide.enabled = false;
    }

    protected virtual void OnFire() { }
    protected virtual void OnHit(Collider2D other) { }
    protected virtual void OnTimeout() { }

    protected bool TryDamage(Collider2D target, float damage)
    {
        IDamagable dam = target.GetComponentInParent<IDamagable>();
        if (dam == null) return false;
        dam.TakeDamage(damage);
        return true;
    }

    void HideLineRenderer(LineRenderer lr)
    {
        lr.enabled = false;
        lr.positionCount = 0;
    }

    public void ResetRigidbody(Rigidbody2D r)
    {
        r.velocity = Vector2.zero;
        r.angularVelocity = 0;
    }
}



public interface IDamagable
{
    void TakeDamage(float amount);
}
public interface IKnockBackable
{
    void ApplyKnockBack(Vector2 direction, float force);
}
