
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlyCameraControl : MonoBehaviour
{
    public float rotationSpeed = 300.0f;
    public float moveSpeed = 5.0f;
    public float speedAdjustmentFactor = 100.0f;
    public float smoothSpeed = 0.01f;

    private float x = 0.0f;
    private float y = 0.0f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();

                x += mouseDelta.x * rotationSpeed * Time.deltaTime;
                y -= mouseDelta.y * rotationSpeed * Time.deltaTime;
            }

            Vector2 scroll = Mouse.current.scroll.ReadValue();

            moveSpeed += scroll.y * 0.01f * speedAdjustmentFactor;
            moveSpeed = Mathf.Max(moveSpeed, 0.1f);
        }

        targetRotation = Quaternion.Euler(y, x, 0);

        float moveHorizontal = 0.0f;
        float moveVertical = 0.0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
            {
                moveHorizontal = -1.0f;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                moveHorizontal = 1.0f;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                moveVertical = -1.0f;
            }

            if (Keyboard.current.wKey.isPressed)
            {
                moveVertical = 1.0f;
            }
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement = transform.TransformDirection(movement);
        targetPosition = transform.position + movement * moveSpeed * Time.deltaTime;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
