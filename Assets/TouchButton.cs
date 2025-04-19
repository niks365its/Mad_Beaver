using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Events;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchButton : MonoBehaviour
{
    public UnityEvent onTouchStart;
    public UnityEvent onTouchEnd;

    private RectTransform rectTransform;
    private Finger currentFinger = null;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += HandleFingerDown;
        Touch.onFingerUp += HandleFingerUp;
        rectTransform = GetComponent<RectTransform>();
    }

    void OnDisable()
    {
        Touch.onFingerDown -= HandleFingerDown;
        Touch.onFingerUp -= HandleFingerUp;
        EnhancedTouchSupport.Disable();
    }

    void HandleFingerDown(Finger finger)
    {
        if (currentFinger != null) return;

        Vector2 screenPoint = finger.screenPosition;
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint))
        {
            currentFinger = finger;
            onTouchStart?.Invoke();
        }
    }

    void HandleFingerUp(Finger finger)
    {
        if (finger == currentFinger)
        {
            onTouchEnd?.Invoke();
            currentFinger = null;
        }
    }
}
