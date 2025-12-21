using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ProjectileController : MonoBehaviour
{
    [SerializeField] protected ProjectileConfig config;

    [Header("Component")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Collider2D collide;
    [SerializeField] protected LineRenderer lineRenderer;

    [SerializeField] protected Vector2 startPosition;
    [SerializeField] protected Vector2 endPosition;
    [SerializeField] protected Vector2 direction;
    protected int ownerLayer;
    protected LayerMask hitMask;

    //Return to pool
    Action<GameObject> returnToPool;

    float lifeTimer;
    protected float lifeTime;

    protected bool isInitialized;
    bool isReturning;
    float distanceFly = 0;
    protected virtual void Start()
    {
        //endPosition = startPosition + direction.normalized * config.maxDistance;
        //lifeTime = distance/speed;

    }
    protected virtual void OnEnable()
    {
        //Reset when get from pool
        CancelInvoke();
        isReturning = false;
        isInitialized = false;
        lifeTimer = 0;
        if (rb != null)
        {
            ResetRigidbody(rb);
        }
        if (lineRenderer != null)
        {
            HideLineRenderer(lineRenderer);
        }

    }
    public void Fire(Vector2 startPoint, Vector2 direction, Vector2 endPoint, int shooterLayer, int mask, float lifeTime, Action<GameObject> returnToPool)
    {
        this.startPosition = startPoint;
        this.endPosition = endPoint;
        this.direction = direction;
        this.ownerLayer = shooterLayer;
        this.hitMask = mask;
        this.lifeTime = lifeTime;
        this.returnToPool = returnToPool;

        isInitialized = true;
        isReturning = false;
        if (lifeTimer >= config.cooldown)
            OnFire();   //Subclass override
    }
    protected virtual void Update()
    {
        if (!isInitialized || isReturning)
        {
            return;
        }

        lifeTimer += Time.deltaTime;
        distanceFly += config.speed * Time.deltaTime;
        if ((lifeTimer >= lifeTime)||(distanceFly >= config.maxDistance))
        {
            lifeTimer = 0;
            OnTimeout();
            ReturnToPool();
            return;
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
    protected virtual bool OnBeforeReturn()
    {
        ResetRigidbody(rb);
        return true;
    }


    protected virtual void OnFire()// start fire, be overriden by subclass
    {


    }
    protected virtual void OnHit(Collider2D other)
    {

    }
    protected virtual void OnTimeout() { }
    protected bool TryDamage(Collider2D target, float damage)
    {
        IDamagable dam = target.GetComponentInParent<IDamagable>();
        if (dam == null) return false;
        dam.TakeDamage(damage);
        return true;
    }

    void HideLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
    }
    public void ResetRigidbody(Rigidbody2D rb)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
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
