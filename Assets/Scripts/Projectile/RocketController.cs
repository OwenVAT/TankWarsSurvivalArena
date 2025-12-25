using UnityEngine;

public class RocketController : ProjectileController
{
    [Header("Curve Settings")]
    
    [SerializeField] private float knockBackForce = 6f; 
    private Vector2 controlPoint;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnFire()
    {
        
        if (rb != null) ResetRigidbody(rb);
        if (collide != null)
        {
            collide.enabled = true;
            collide.isTrigger = true;
        }
        transform.position = startPosition;
        controlPoint = (startPosition + endPosition) * 0.5f + config.arcHeight * Utilities.GetPerpendicularUp(startPosition, endPosition);
    }

    protected override void Update()
    {
        if (!isInitialized || isReturning) return;

        base.Update();

        float t = Mathf.Clamp01(lifeTimer/lifeTime);

        Vector2 pos = Utilities.QuadraticBezier(startPosition, controlPoint, endPosition, t);
        //Vector2 pos = Utilities.DiagonalFly(startPosition, config.speed*direction,-100f* Utilities.GetPerpendicularUp(startPosition,endPosition) ,lifeTimer);
        transform.position = pos;
        Debug.Log("t: " + t);
        Debug.Log("pos: " + pos);

        // Rotate to direction movement
       // float t2 = t + Time.deltaTime;
       //// Vector2 next = Utilities.QuadraticBezier(startPosition, controlPoint, endPosition, t2);
       // Vector2 next = Utilities.DiagonalFly(startPosition, config.speed * direction, -1f * Utilities.GetPerpendicularUp(startPosition, endPosition), t2);
       // Vector2 dir = next - pos;
       // if (dir.sqrMagnitude > 0.0001f)
       //     transform.up = dir.normalized;

     
        if (lifeTimer >= lifeTime)
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
      
    }
}
