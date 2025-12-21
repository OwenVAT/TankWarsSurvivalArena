using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankController : TankController
{
    [Header("Input Setting")]
    public Joystick moveJoystick;
    public Joystick aimJoystick;
    public bool autoAimWithoutAimJoystick = true;
    //public Button fireButton;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        HandleMobileInput();
    }

    private void HandleMobileInput()
    {
            Vector2 move = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);
            SetMoveInput(move);
        //}
        Vector2 aimInput = Vector2.zero;
        if (aimJoystick != null)
        {
            aimInput = new Vector2(aimJoystick.Horizontal, aimJoystick.Vertical);
        }
        if (autoAimWithoutAimJoystick && aimInput.sqrMagnitude==0) 
        {
            aimInput = move;
        }
        SetAimDirection(aimInput);
    }
   
}
