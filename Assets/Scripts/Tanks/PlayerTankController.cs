using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class PlayerTankController : TankController
{
    [Header("Input Setting")]
    public bool autoAimWithoutAimJoystick = true;
    bool isFire = false;
    float lifeTimer = 0;
    float powerFireRocket;
    bool rocketButtonPressed = false;



    //    [SerializeField] private Joystick rocketJoystick;
    [SerializeField] private TrajectoryRenderer trajectory;
    [SerializeField] private ProjectileConfig rocketConfig;

    bool isAiming;
    ProjectileType lastProjectile;


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        SetMoveInput(GetInputJoystick.Instance.MoveInput());
        SetAimDirection(GetInputJoystick.Instance.AimInput(), out power);

        if (rocketButtonPressed && (currentProjectile != ProjectileType.Rocket))
        {
            lastProjectile = currentProjectile;
            ChangeWeapon(ProjectileType.Rocket);
        }
        if (rocketButtonPressed)
        {
            SetUpFire(rocketConfig,out start, out dir, out  end,power);
            trajectory.DrawCurve(start, end);
            isAiming= true;
        }
        else
        {
            if (isAiming)
            {
                // fire when put joystick up
                trajectory.gameObject.GetComponent<LineRenderer>().enabled = false;
                isFire=true;
                lifeTimer = 0;
            }
            isAiming = false;
        }


        if ((isFire)&&(lifeTimer<rocketConfig.lifeTime)) 
        { 
            lifeTimer+= Time.deltaTime;
            TryFire(); 
        }
        if (lifeTimer >= rocketConfig.lifeTime) 
        { 
            isFire = false;
            lifeTimer = 0;
            return;
        }
    }
    public void OnFireButtonPressed()
    {
        if (currentProjectile == ProjectileType.Rocket)
        {
            currentProjectile = lastProjectile;
        }
        TryFire();
    }
    public void OnFireRocketButtonUp(Vector2 lastVector)
    {
        powerFireRocket = Mathf.Clamp01(lastVector.magnitude);
        SetUpFire(rocketConfig, out start, out dir, out end, powerFireRocket);
        isFire = true;
        rocketButtonPressed = false;
    }
    public void OnFireRocketButtonUp_NoArg()
    {
        Vector2 lastVector = GetInputJoystick.Instance.AimInput();
        OnFireRocketButtonUp(lastVector);
    }


    public void OnFireRocketButtonPressed()
    {
        rocketButtonPressed= true;
    }


}
