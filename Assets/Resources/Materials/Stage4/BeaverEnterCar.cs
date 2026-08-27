using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BeaverEnterCar : MonoBehaviour
{
    [Header("Об'єкти")]
    public Transform EnterPoint;
    public Transform Beaver;

    private Input input;

    [Header("Налаштування")]
    public float moveDuration = 1.0f;

    private bool beaverInArea = false;
    private bool isMoving = false;

    private void Awake()
    {
        input = new Input();

        input.player.Throw.performed += moveToPoint;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void moveToPoint(InputAction.CallbackContext context)
    {
        Debug.Log("Throw натиснуто");

        if (beaverInArea && !isMoving)
        {
            StartCoroutine(MoveBeaverToEnterPoint());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("У SenseArea увійшов: " + other.name);

        if (other.transform.root == Beaver)
        {
            beaverInArea = true;
            Debug.Log("БОБЕР УВІЙШОВ У SENSE AREA");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Із SenseArea вийшов: " + other.name);

        if (other.transform.root == Beaver)
        {
            beaverInArea = false;
            Debug.Log("БОБЕР ВИЙШОВ ІЗ SENSE AREA");
        }
    }

    private IEnumerator MoveBeaverToEnterPoint()
    {
        isMoving = true;

        Vector3 startPosition = Beaver.position;
        Vector3 targetPosition = EnterPoint.position;

        Quaternion targetRotation = EnterPoint.rotation;

        float time = 0f;

        // Напрямок руху до EnterPoint
        Vector3 moveDirection = (targetPosition - startPosition).normalized;

        // Поворот, щоб ніс бобра дивився в напрямку руху
        Quaternion moveRotation = Quaternion.LookRotation(moveDirection, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);

        // 80% часу — рух до EnterPoint
        float moveDurationPart = moveDuration * 0.8f;

        while (time < moveDurationPart)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / moveDurationPart);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Рух
            Beaver.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            // Ніс дивиться в напрямку руху
            Beaver.rotation = moveRotation;

            yield return null;
        }

        // Точно ставимо в EnterPoint
        Beaver.position = targetPosition;

        // 20% часу — довертання на місці
        float rotateTime = 0f;
        float rotateDuration = moveDuration * 0.2f;

        while (rotateTime < rotateDuration)
        {
            rotateTime += Time.deltaTime;

            float t = Mathf.Clamp01(rotateTime / rotateDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            Beaver.rotation = Quaternion.Slerp(
                moveRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        // Фінальне положення
        Beaver.position = targetPosition;
        Beaver.rotation = targetRotation;

        isMoving = false;

        Debug.Log("Бобер доїхав до EnterPoint і повернувся у потрібне положення");
    }
}