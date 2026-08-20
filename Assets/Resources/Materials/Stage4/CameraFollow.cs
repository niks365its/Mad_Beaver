// using UnityEngine;

// public class CameraFollow : MonoBehaviour
// {
//     public Transform target;
//     public Vector3 offset;

//     void LateUpdate()
//     {
//         transform.position = target.position + offset;
//     }
// }

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    [Header("Camera Rotation")]
    public float rotationSmoothTime = 0.3f;

    private float currentY;
    private float rotationVelocity;

    void Start()
    {
        currentY = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        // Камера слідує за бобром
        transform.position = target.position + offset;

        // Напрямок бобра
        float targetY = target.eulerAngles.y;

        // Плавне довертання камери із запізненням
        currentY = Mathf.SmoothDampAngle(
            currentY,
            targetY,
            ref rotationVelocity,
            rotationSmoothTime
        );

        // Зберігаємо X і Z камери
        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            currentY,
            transform.eulerAngles.z
        );
    }
}