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

    public bool beaverInArea = false;
    public bool isMoving = false;

    public bool inAvto = false;


    private Coroutine blinkCoroutine;


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

        if (!beaverInArea || isMoving) return;

        if (!inAvto)
        {
            StartCoroutine(MoveBeaverToEnterPoint());
        }

        else //(beaverInArea && !isMoving && inAvto)
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


            // doorShineObject.SetActive(true);
            if (blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkDoorShine());
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Із SenseArea вийшов: " + other.name);

        if (other.transform.root == Beaver.root)
        {
            beaverInArea = false;
            Debug.Log("БОБЕР ВИЙШОВ ІЗ SENSE AREA");

            //  doorShineObject.SetActive(false);

            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
                doorShineObject.SetActive(false);
            }
        }


    }

    private IEnumerator MoveBeaverToEnterPoint()
    {
        Debug.Log("Sit Animation, isMoving =  " + isMoving + " inAvto = " + inAvto + "beaverInArea " + beaverInArea);
        isMoving = true;
        inAvto = false;
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

        //doorShineObject.SetActive(false);

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            doorShineObject.SetActive(false);
        }

        if (Vector3.Distance(startPosition, targetPosition) > 0.5f)
        {
            Debug.Log("Go Animation, isMoving =  " + isMoving + " inAvto = " + inAvto + "beaverInArea " + beaverInArea);
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

        }

        animator.SetBool("IsGo", false);

        animator.SetBool("IsOutOfCar", false);
        animator.SetBool("IsSitToCar", true);



        Rigidbody rb = Beaver.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Transform beaverCollider = Beaver.Find("Colliders");
        if (beaverCollider != null)
        {
            beaverCollider.gameObject.SetActive(false);
        }

        Beaver.SetParent(avto.transform, true);
        inAvto = true;

        yield return new WaitForSeconds(3f);


        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

        if (cameraFollow != null)
        {
            cameraFollow.target = avto.transform;
        }

        avto.GetComponent<MovingCar>().enabled = true;
        animator.SetBool("IsSitToCar", false);
        isMoving = false;
        beaverInArea = true;
        Debug.Log("Бобер доїхав до EnterPoint і повернувся у потрібне положення");
    }

    private IEnumerator MoveBeaverFromAvto()
    {
        Debug.Log("Out Animation, isMoving =  " + isMoving + " inAvto = " + inAvto + "beaverInArea " + beaverInArea);
        isMoving = true;

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.target = Beaver;
        }

        avto.GetComponent<MovingCar>().enabled = false;

        animator.SetBool("IsOutOfCar", true);



        yield return new WaitForSeconds(3f);

        Beaver.SetParent(null, true);

        Beaver.GetComponent<Moving>().enabled = true;
        inAvto = false;
        isMoving = false;

        animator.SetBool("IsOutOfCar", false);

        Transform beaverCollider = Beaver.Find("Colliders");
        if (beaverCollider != null)
        {
            beaverCollider.gameObject.SetActive(true);
        }

        Rigidbody rb = Beaver.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.detectCollisions = true;
            rb.isKinematic = false;
        }
    }

    private IEnumerator BlinkDoorShine()
    {
        while (true)
        {
            doorShineObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            doorShineObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}