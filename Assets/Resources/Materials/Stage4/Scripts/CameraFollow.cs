using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float high = 1.5f;

    [Header("Rotation")]
    public float rotationSmoothTime = 0.4f;

    [Header("Movement detection")]
    [Tooltip("Minimum speed (units/sec) before the camera follows movement direction instead of facing direction.")]
    public float minMoveSpeed = 0.1f;

    [Header("Idle Camera Rotation")]
    [Tooltip("Скільки секунд чекати перед початком обертання.")]
    public float idleTimeBeforeRotation = 10f;

    [Tooltip("Швидкість автоматичного обертання камери навколо цілі.")]
    public float idleRotationSpeed = 5f;

    [Tooltip("Мінімальна зміна повороту цілі, яка вважається рухом.")]
    public float rotationThreshold = 0.5f;

    private float currentY;
    private float rotationVelocity;
    private Vector3 lastPosition;
    private float lastTargetY;
    private float idleTimer;

    void Start()
    {
        currentY = target.eulerAngles.y;
        lastTargetY = target.eulerAngles.y;
        lastPosition = target.position;
    }

    void LateUpdate()
    {
        // -----------------------------
        // Визначаємо рух
        // -----------------------------

        Vector3 moveDelta = target.position - lastPosition;
        moveDelta.y = 0f;
        lastPosition = target.position;

        float speed = moveDelta.magnitude / Time.deltaTime;

        // -----------------------------
        // Визначаємо поворот цілі
        // -----------------------------

        float currentTargetY = target.eulerAngles.y;

        float rotationChange = Mathf.Abs(
            Mathf.DeltaAngle(lastTargetY, currentTargetY)
        );

        lastTargetY = currentTargetY;

        bool isMoving = speed > minMoveSpeed;
        bool isRotating = rotationChange > rotationThreshold;

        // -----------------------------
        // Таймер простою
        // -----------------------------

        if (isMoving || isRotating)
        {
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        // -----------------------------
        // Визначаємо напрямок камери
        // -----------------------------

        float targetY;

        if (idleTimer >= idleTimeBeforeRotation)
        {
            // Бобер стоїть 10+ секунд.
            // Починаємо повільно обертати камеру навколо нього.

            currentY += idleRotationSpeed * Time.deltaTime;

            targetY = currentY;
        }
        else if (isMoving)
        {
            // Бобер рухається — камера дивиться
            // у напрямку фактичного руху.

            float moveAngle = Mathf.Atan2(
                moveDelta.x,
                moveDelta.z
            ) * Mathf.Rad2Deg;

            targetY = moveAngle + 90f;
        }
        else
        {
            // Бобер стоїть — камера тримається
            // напрямку, куди він дивиться.

            targetY = target.eulerAngles.y - 90f;
        }

        // -----------------------------
        // Плавний поворот
        // -----------------------------

        if (idleTimer < idleTimeBeforeRotation)
        {
            currentY = Mathf.SmoothDampAngle(
                currentY,
                targetY,
                ref rotationVelocity,
                rotationSmoothTime
            );
        }

        // -----------------------------
        // Позиція камери
        // -----------------------------

        Quaternion rotation = Quaternion.Euler(
            0f,
            currentY,
            0f
        );

        Vector3 newPosition =
            target.position + rotation * offset;

        transform.position = newPosition;

        // Камера дивиться на бобра
        transform.LookAt(
            target.position + Vector3.up * high
        );
    }
}