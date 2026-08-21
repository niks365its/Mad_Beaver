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

    [Header("Rotation")]
    public float rotationSmoothTime = 0.4f;

    private float currentY;
    private float rotationVelocity;

    void Start()
    {
        currentY = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        // Напрямок бобра
        float targetY = target.eulerAngles.y - 90f;

        // Плавно наздоганяємо поворот бобра
        currentY = Mathf.SmoothDampAngle(
            currentY,
            targetY,
            ref rotationVelocity,
            rotationSmoothTime
        );

        // Повертаємо offset навколо бобра
        Quaternion rotation = Quaternion.Euler(0f, currentY, 0f);

        Vector3 newPosition = target.position + rotation * offset;

        transform.position = newPosition;

        // Камера дивиться на бобра
        transform.LookAt(target);
    }
}