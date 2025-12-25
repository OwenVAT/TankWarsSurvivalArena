using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JoystickHandleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Joystick joystick;
    public RectTransform handle;
    [System.Serializable] public class Vector2Event : UnityEngine.Events.UnityEvent<Vector2> { }
    public Vector2Event OnHandleUp;
    public UnityEvent OnHandleDown;



    Vector2 lastVector;

   
    public bool isHandlePressed {  get; private set; }
  
 
    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(handle, eventData.position, eventData.pressEventCamera))
        {
            isHandlePressed = true;
            lastVector = Vector2.zero;
            OnHandleDown?.Invoke();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {        
        lastVector = GetInputJoystick.Instance.AimInput();
        isHandlePressed = false;
        OnHandleUp?.Invoke(lastVector);
        
    }

}
