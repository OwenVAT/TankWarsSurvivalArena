using UnityEngine;

[RequireComponent(typeof(TankWeaponSystem))]
public class PlayerTankController : TankController
{
    [Header("Rocket Aim")]
    [SerializeField] private JoystickHandleButton aimHandleButton;
    [SerializeField] private TrajectoryRenderer trajectory;
    [SerializeField] private ProjectileConfig rocketConfig;
    [SerializeField] private float rocketGravity = 10f;

    private TankWeaponSystem weapon;
    private bool aimingRocket;

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponent<TankWeaponSystem>();
    }

    protected override void Start()
    {
        base.Start();

        if (aimHandleButton != null)
        {
            aimHandleButton.OnHandleDown.AddListener(IsAiming);
            aimHandleButton.OnHandleUp.AddListener(OnRocketHandleUp);
        }
    }

    protected override void Update()
    {
        if (!isDead)
        {
            SetMoveInput(GetInputJoystick.Instance.MoveInput());
            SetAimDirection(GetInputJoystick.Instance.AimInput());
        }

        base.Update();

        if (!isDead && aimingRocket)
        {
            Vector2 aim = GetInputJoystick.Instance.AimInput();
            float p = Mathf.Clamp01(aim.magnitude);
            if (p > 0.02f)
            {
                Vector2 start = firePoint.position;
                Vector2 v0 = aim.normalized * rocketConfig.speed * p;
                Vector2 a = Vector2.down * rocketGravity;
                trajectory.DrawCurve(start, v0, a, rocketConfig.lifeTime, Time.deltaTime);
            }
        }
    }

    public void OnFireButtonPressed()
    {
        if (isDead) return;
        Vector2 dir = aimDirection.sqrMagnitude > 0.0001f ? aimDirection : (Vector2)turretTransform.up;
        weapon.FirePrimary(dir);
        shootAnimator.SetTrigger(shootAnim_Param);

    }

    private void OnRocketHandleUp()
    {
        if (isDead) return;

        aimingRocket = false;
        if (trajectory != null) trajectory.Hide();

        Vector2 aim = GetInputJoystick.Instance.AimInput();
        float p = Mathf.Clamp01(aim.magnitude);
        if (p <= 0.05f) 
            return;

        weapon.FireRocket(aim, p);
    }
    private void IsAiming()
    {
        aimingRocket = true;
    }
}
