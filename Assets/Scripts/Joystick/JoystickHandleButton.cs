using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JoystickHandleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform handle;
    public UnityEvent OnHandleUp;
    public UnityEvent OnHandleDown;

    public bool isHandlePressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(handle, eventData.position, eventData.pressEventCamera))
        {
            isHandlePressed = true;
            OnHandleDown?.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHandlePressed = false;
        OnHandleUp?.Invoke();
    }
}
