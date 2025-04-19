using UnityEngine;

public class FixedJoystickAdapter : MonoBehaviour
{
    public FixedJoystick joystick;
    public float threshold = 0.2f;
    public Control control; // твій скрипт з TouchLeft тощо

    private bool isLeftActive = false;
    private bool isRightActive = false;
    private bool isJumpActive = false;


    void Update()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        if (h < -threshold)
        {
            if (!isLeftActive)
            {
                control.TouchLeft();
                isLeftActive = true;
            }
        }
        else
        {
            if (isLeftActive)
            {
                control.UnTouchLeft();
                isLeftActive = false;
            }
        }

        if (h > threshold)
        {
            if (!isRightActive)
            {
                control.TouchRight();
                isRightActive = true;
            }
        }
        else
        {
            if (isRightActive)
            {
                control.UnTouchRight();
                isRightActive = false;
            }
        }

        if (v > threshold)
        {

            control.TouchJump();


        }
    }
}
