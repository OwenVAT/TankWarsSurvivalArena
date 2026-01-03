using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class TankController : MonoBehaviour, IDamagable, IKnockBackable
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Transform bodyTransform;
    public Transform turretTransform;
    public Transform firePoint;

    [Header("Visual")]
    [SerializeField] protected SpriteRenderer[] renderersToFlash;
    [SerializeField] protected Animator shootAnimator; // Trigger "isShooting"
    [SerializeField] protected Animator tankExplodeAnimator;   // Trigger "Explode"
    [SerializeField] protected Animator leftTrackAnimator;  // Float "speedTrack"
    [SerializeField] protected Animator rightTrackAnimator; // Float "speedTrack"
    protected string shootAnim_Param = "isShooting"; 
    protected string trackAnim_Param = "speedTrack";
    protected string explode_anim="explode";

    [Header("UI")]
    [SerializeField] protected TankHealthBarUI healthBarUI;

    [Header("Config")]
    public TankConfig baseConfig;

    [Header("Runtime")]
    [SerializeField] protected float currentHP;
    [SerializeField] protected float currentArmor;
    [SerializeField] protected float moveSpeed;

    [Header("Weapons")]
    [SerializeField] protected ProjectileType primaryWeapon = ProjectileType.LightShell;
    [SerializeField] protected ProjectileType rocketWeapon = ProjectileType.Rocket;

    protected Vector2 moveInput;
    protected Vector2 aimDirection;
    protected bool isDead;

    protected virtual void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    //    renderersToFlash=GetComponentsInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        ApplyConfig();
        if (healthBarUI != null) healthBarUI.BindHealthBarToTank(transform);
        UpdateHealthUI();
    }

    protected virtual void Update()
    {
        if (isDead) return;
        RotateTurret();
        UpdateTrackAnim();
    }

    protected virtual void FixedUpdate()
    {
        if (isDead) return;
        Move();
    }

    protected virtual void ApplyConfig()
    {
        if (baseConfig == null) return;
        currentHP = baseConfig.maxHP;
        currentArmor = baseConfig.armor;
        moveSpeed = baseConfig.moveSpeed;
        primaryWeapon = baseConfig.primaryWeapon;
        rocketWeapon = baseConfig.rocketWeapon;
    }

    public ProjectileType GetPrimaryWeapon()
    {
        return primaryWeapon;
    }
    public ProjectileType GetRocketWeapon()
    {
        return rocketWeapon;
    }

    public float GetHP()
    {
        return currentHP;
    }
    public float GetArmor()
    {
        return currentArmor;
    }
    public bool IsDead()
    {
        return isDead;
    }

    public void SetHPArmor(float hp, float armor)
    {
        currentHP = hp;
        currentArmor = armor;
        UpdateHealthUI();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = Vector2.ClampMagnitude(input, 1f); 
    }  
    public void SetAimDirection(Vector2 aim)
    {
        aimDirection = aim;
    }

    protected virtual void Move()
    {
        rb.velocity = moveInput * moveSpeed;

        if (moveInput.sqrMagnitude > 0.0001f && bodyTransform != null)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            bodyTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected virtual void RotateTurret()
    {
        if (turretTransform == null) return;

        Vector2 dir = aimDirection.sqrMagnitude > 0.0001f ? aimDirection : moveInput;
        if (dir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            turretTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected virtual void UpdateTrackAnim()
    {
        float speed01 = Mathf.Clamp01(rb.velocity.magnitude / Mathf.Max(0.01f, moveSpeed));
        leftTrackAnimator.SetFloat(trackAnim_Param, speed01);
        rightTrackAnimator.SetFloat(trackAnim_Param, speed01);
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        float damageAfterArmor = Mathf.Max(amount - currentArmor, 1f);
        currentHP -= damageAfterArmor;

        FlashRed();
        UpdateHealthUI();

        if (currentHP <= 0f) Die();
    }

    public virtual void Heal(float amount)
    {
        if (isDead) return;
        float maxHP = baseConfig != null ? baseConfig.maxHP : 100f;
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        FlashGold();
        UpdateHealthUI();
    }

    public virtual void AddArmor(float amount)
    {
        if (isDead) return;
        currentArmor += amount;
        FlashGold();
    }

    public void ApplyKnockBack(Vector2 direction, float force)
    {
        if (isDead) return;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    protected void FlashRed()
    {
        StartCoroutine(FlashColor(Color.red, 0.12f));
        if (healthBarUI != null) healthBarUI.Flash(Color.red, 0.12f);
    }

    protected void FlashGold()
    {
        Color gold = new Color(1f, 0.85f, 0.2f);
        StartCoroutine(FlashColor(gold, 0.15f));
        healthBarUI.Flash(gold, 0.15f);
    }

    protected IEnumerator FlashColor(Color c, float duration)
    {
        if (renderersToFlash == null || renderersToFlash.Length == 0) 
            yield break;

        Color[] old = new Color[renderersToFlash.Length];
        for (int i = 0; i < renderersToFlash.Length; i++)
        {
            if (renderersToFlash[i] == null) continue;
            old[i] = renderersToFlash[i].color;
            renderersToFlash[i].color = c;
        }

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < renderersToFlash.Length; i++)
        {
            if (renderersToFlash[i] == null) 
                continue;
            renderersToFlash[i].color = old[i];
        }
    }

    protected void UpdateHealthUI()
    {
        if (healthBarUI == null) return;
        float maxHP = baseConfig != null ? baseConfig.maxHP : 100f;
        healthBarUI.SetHealth_UI(currentHP, maxHP);
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.velocity = Vector2.zero;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (tankExplodeAnimator != null)
        {
            tankExplodeAnimator.SetTrigger("Explode");
            Destroy(gameObject, 0.9f);
        }
        else Destroy(gameObject);
    }
}
