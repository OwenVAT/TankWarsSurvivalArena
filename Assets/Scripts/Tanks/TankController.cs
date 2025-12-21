using System;
using System.Collections;
using System.Collections.Generic;
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
    protected float rotateSpeed;
    protected int fireRate;
    [SerializeField]protected ProjectileType currentProjectile;

    [Header("WeaponType Prefabs")]
    public GameObject bulletPrefab;
    public GameObject laserPrefab;
    public GameObject rocketPrefab;
    
    
    protected Vector2 moveInput; // direction of move
    protected Vector2 aimDirection; //direction of turret

    protected float lastFireTime = float.MinValue;

    protected virtual void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //khởi tạo stats
        if (baseConfig != null)
        {
            currentHP = baseConfig.maxHP;
            currentArmor = baseConfig.armor;
            moveSpeed = baseConfig.moveSpeed;
            fireRate = baseConfig.fireRate;
            currentProjectile = baseConfig.defaultProjectile;
        }
    }

    //-------UPDATE & FIXEDUPDATE--------
    // Update is called once per frame
    protected virtual void Update()
    {
        RotateTurret();
    }
    protected virtual void FixedUpdate()
    {
        Move();
    }

    //-------Move Tank & Roate Turret-------
    public void SetMoveInput(Vector2 input)
    {
        moveInput = Vector2.ClampMagnitude(input, 1f);
    }

    public void SetAimDirection(Vector2 aim)
    {
        aimDirection = aim;
    }
    protected void Move()
    {
        //move tank hull base on moveInput
        rb.velocity = moveInput * moveSpeed;
        //rotate the tank toward the mobeInput direction
        if (moveInput.sqrMagnitude > 0)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg -90f;
            //float angle = Mathf.LerpAngle(bodyTransform.eulerAngles.z, targetAngle, rotateSpeed * Time.fixedDeltaTime);
            bodyTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    protected void RotateTurret()
    {
        Vector2 dir = aimDirection.magnitude > 0 ? aimDirection : moveInput;
        if (dir.sqrMagnitude > 0)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            turretTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    //--------Shoot---------
    public virtual void TryFire()
    {
        if (Time.time < (lastFireTime + 1f / fireRate))
        {
            return;
        }
        Fire();
        lastFireTime = Time.time;
    }
    protected virtual void Fire()
    {
        GameObject prefabToSpawn = null;
        switch (currentProjectile)
        {
            case ProjectileType.LightShell:
                prefabToSpawn = bulletPrefab;
                break;
            case ProjectileType.Laser:
                prefabToSpawn = laserPrefab;
                break;
            case ProjectileType.Rocket:
                prefabToSpawn = rocketPrefab;
                break;
        }
        GameObject bullet = Instantiate(prefabToSpawn, firePoint.position, firePoint.rotation);
        var f = bullet.GetComponent<ProjectileController>();
       // f.Fire(firePoint.position,firePoint.transform.up,)
    }
    //-------------Damge & Heal--------
    public virtual void TakeDamage(float amount)
    {
        float damageAfterArmor = Mathf.Max(amount - currentArmor, 1f);
        currentHP -= damageAfterArmor;
        OnHit();
        if (currentHP <= 0)
        {
            Die();
        }
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

    //----------Effect----------
    protected virtual void OnHit() { }
    protected virtual void Die() 
    {
        Destroy(gameObject);
    }
}
