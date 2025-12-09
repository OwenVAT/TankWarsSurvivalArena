using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public Joystick moveJoystick;
    public Joystick aimJoystick;
    public float speedMove = 2f;
    public Transform Tower;
    public Animator leftTrack;
    public Animator rightTrack;

    Rigidbody2D rb;
    Vector2 move,aim;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        move = GetInputMoveJoystick();
        aim = GetInputAimJoystick();
        UpdateTrackAnimation();
    }
    void FixedUpdate()
    {
        MoveTank(move);
        TurretRotate(aim);
    }



    private void UpdateTrackAnimation()
    {
        float speed = move.magnitude;
        leftTrack.SetFloat("speedTrack", speed);
        rightTrack.SetFloat("speedTrack", speed);
    }

  

    private void TurretRotate(Vector2 aim)
    {
        //float angle = Mathf.Atan2(aim.y, aim.x)*Mathf.Rad2Deg;
        if (aim.magnitude > 0)
        {
            Tower.transform.rotation = Quaternion.LookRotation(Vector3.forward, aim);
        }
    }

    private void MoveTank(Vector2 move)
    {
        if (move.magnitude > 0)
        {
            float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;

        }
        rb.MovePosition(rb.position + move * speedMove * Time.fixedDeltaTime);
    }

    private Vector2 GetInputMoveJoystick()
    {
        return new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);
    }
    private Vector2 GetInputAimJoystick()
    {
        return new Vector2(aimJoystick.Horizontal, aimJoystick.Vertical);
    }



}
