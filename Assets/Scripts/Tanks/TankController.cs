using System;
using UnityEngine;

public abstract class TankController : MonoBehaviour, IDamagable
{
    [Header("Component")]
    public Rigidbody2D rb;
    public Transform bodyTransform;
    public Transform turretTransform;
    public Transform firePoint;

    [Header("Stats Config")]
    public TankConfig baseConfig;

    protected float currentHP;
    protected float currentArmor;
    protected float moveSpeed;
    protected float cooldown;

    [SerializeField] protected ProjectileType currentProjectile;

    protected Vector2 moveInput;
    protected Vector2 aimDirection;
    protected float power; //ratio of range of handle to max range of handle 

    protected float lastFireTime = -99f;
    protected Vector2 start, dir, end;

    protected virtual void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        if (baseConfig != null)
        {
            currentHP = baseConfig.maxHP;
            currentArmor = baseConfig.armor;
            moveSpeed = baseConfig.moveSpeed;
            cooldown = baseConfig.cooldown;
            currentProjectile = baseConfig.defaultProjectile;
        }
    }

    protected virtual void Update()
    {
        RotateTurret();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = Vector2.ClampMagnitude(input, 1f);
    }
    public void SetAimDirection(Vector2 aim, out float powerToFire)
    {
        aimDirection = aim;
        powerToFire = Mathf.Clamp01(aimDirection.magnitude);
    }

    protected void Move()
    {
        rb.velocity = moveInput * moveSpeed;

        if (moveInput.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            bodyTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected void RotateTurret()
    {
        Vector2 dir = aimDirection.sqrMagnitude > 0 ? aimDirection : moveInput;
        if (dir.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            turretTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public virtual void TryFire()
    {
        var pCfg = ProjectileDatabase.Instance != null
            ? ProjectileDatabase.Instance.GetProjectileConfig(currentProjectile)
            : null;

        if (Time.time < lastFireTime + cooldown) return;

        FireProjectile(pCfg);
        lastFireTime = Time.time;
    }

    protected virtual void FireProjectile(ProjectileConfig pCfg)
    {
        GameObject projObj = PoolManager.Instance.GetProjectile(currentProjectile);
        if (projObj == null) return;

        projObj.transform.position = firePoint.position;
        projObj.transform.rotation = firePoint.rotation;

        ProjectileController proj = projObj.GetComponent<ProjectileController>();
        if (proj == null)
        {
            projObj.SetActive(false);
            return;
        }        
        proj.Fire(start, dir, end, gameObject.layer, pCfg.hitMask, pCfg.lifeTime, (go) => PoolManager.Instance.ReturnProjectile(currentProjectile, go));
    }

    protected void SetUpFire(ProjectileConfig pCfg, out Vector2 start, out Vector2 dir, out Vector2 end, float powerToFire)
    {
        start = firePoint.position;
        dir = firePoint.up;
        end = start + dir.normalized * pCfg.maxDistance * powerToFire;
    }

    public virtual void TakeDamage(float amount)
    {
        float damageAfterArmor = Mathf.Max(amount - currentArmor, 1f);
        currentHP -= damageAfterArmor;
        OnHit();
        if (currentHP <= 0)
            Die();
    }

    public virtual void Heal(float amount)
    {
        currentHP += amount;
    } 
    public virtual void AddAmor(float amount)
    {
        currentArmor += amount;
    } 
    public virtual void ChangeWeapon(ProjectileType newWeapon)
    {
        currentProjectile = newWeapon;
    }
    protected virtual void OnHit() { }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
