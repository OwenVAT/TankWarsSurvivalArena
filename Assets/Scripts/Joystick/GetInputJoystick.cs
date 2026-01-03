using UnityEngine;

public class GetInputJoystick : MonoBehaviour
{
    public static GetInputJoystick Instance;
    public Joystick moveJoystick;
    public Joystick aimJoystick;

    public Vector2 MoveInput() => new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);
    public Vector2 AimInput() => new Vector2(aimJoystick.Horizontal, aimJoystick.Vertical);

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
