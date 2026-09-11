using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    public float rotationSpeed = 200.0f;
    public float smoothSpeed = 0.01f; // The speed of the smooth movement
    public float zoomSpeed = 5.0f; // The speed of the zoom

    public float minY = 1.0f; // Replace with your desired minimum Y angle
    public float maxY = 100.0f; // Replace with your desired maximum Y angle


    private float x = 0.0f;
    private float y = 0.0f;
    private Quaternion targetRotation;
    private Vector3 targetPosition;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Initialize the target rotation and position
        targetRotation = Quaternion.Euler(y, x, 0);
        targetPosition = targetRotation * new Vector3(0.0f, 0.0f, distance) + target.position;

        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
        {
            Camera.main.depthTextureMode = DepthTextureMode.Depth;
        }
        else
        {
            Debug.LogError("Depth textures are not supported on this device!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                x += mouseDelta.x * rotationSpeed * Time.deltaTime;
                float yRotation = y - mouseDelta.y * rotationSpeed * Time.deltaTime;

                // Clamp the Y rotation between the min and max values
                yRotation = Mathf.Clamp(yRotation, minY, maxY);
                y = yRotation;
            }

            targetRotation = Quaternion.Euler(y, x, 0);

            Vector2 scroll = Mouse.current.scroll.ReadValue();
            distance = Mathf.Clamp(distance - scroll.y * zoomSpeed, minDistance, maxDistance);
            targetPosition = targetRotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        }
    }



    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        // Interpolate the current rotation and position to the target rotation and position
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}










