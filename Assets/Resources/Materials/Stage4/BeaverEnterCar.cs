using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BeaverEnterCar : MonoBehaviour
{
    [Header("Об'єкти")]
    public Transform EnterPoint;
    public Transform Beaver;

    public GameObject avto;

    public Animator animator;

    public GameObject doorShineObject;

    private Input input;

    [Header("Налаштування")]
    public float moveDuration = 1.0f;

    private bool beaverInArea = false;
    private bool isMoving = false;

    private bool inAvto = false;





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

        if (beaverInArea && !isMoving && !inAvto)
        {
            StartCoroutine(MoveBeaverToEnterPoint());
        }

        if (beaverInArea && !isMoving && inAvto)
        {
            StartCoroutine(MoveBeaverFromAvto());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("У SenseArea увійшов: " + other.name);

        if (other.transform.root == Beaver.root)
        {
            beaverInArea = true;
            Debug.Log("БОБЕР УВІЙШОВ У SENSE AREA");


            doorShineObject.SetActive(true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Із SenseArea вийшов: " + other.name);

        if (other.transform.root == Beaver.root)
        {
            beaverInArea = false;
            Debug.Log("БОБЕР ВИЙШОВ ІЗ SENSE AREA");

            doorShineObject.SetActive(false);
        }


    }

    private IEnumerator MoveBeaverToEnterPoint()
    {
        isMoving = true;

        Beaver.GetComponent<Moving>().enabled = false;

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
            animator.SetBool("IsGo", true);
            // Ніс дивиться в напрямку руху
            Beaver.rotation = moveRotation;

            yield return null;
        }
        doorShineObject.SetActive(false);
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
        animator.SetBool("IsGo", false);


        animator.SetBool("IsSitToCar", true);

        avto.GetComponent<MovingCar>().enabled = true;

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

        if (cameraFollow != null)
        {
            cameraFollow.target = avto.transform;
        }

        Beaver.SetParent(avto.transform, true);
        inAvto = true;
        isMoving = false;
        yield return new WaitForSeconds(3f);
        animator.SetBool("IsSitToCar", false);
        Debug.Log("Бобер доїхав до EnterPoint і повернувся у потрібне положення");
    }

    private IEnumerator MoveBeaverFromAvto()
    {
        isMoving = true;
        avto.GetComponent<MovingCar>().enabled = false;

        animator.SetBool("IsOutOfCar", true);

        yield return new WaitForSeconds(3f);

        Beaver.SetParent(avto.transform, false);
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

        if (cameraFollow != null)
        {
            cameraFollow.target = Beaver;
        }
        Beaver.GetComponent<Moving>().enabled = true;
        inAvto = false;
        isMoving = false;

        animator.SetBool("IsOutOfCar", false);
    }
}