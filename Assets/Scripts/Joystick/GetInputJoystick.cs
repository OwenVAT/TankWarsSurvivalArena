using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetInputJoystick : MonoBehaviour
{
    public static GetInputJoystick Instance;
    public Joystick moveJoystick;
    public Joystick aimJoystick;
    public Vector2 VectorInput(Joystick joystick) 
    {
        return new Vector2(joystick.Horizontal, joystick.Vertical);
    }
    public Vector2 MoveInput()
    {
        return VectorInput(moveJoystick);
    }

    public Vector2 AimInput()
    {
        return VectorInput(aimJoystick);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
