using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.AI;

public class BeaverEnterCar : MonoBehaviour
{
    [Header("Objects")]
    public Transform EnterPoint;
    public Transform Beaver;

    public GameObject avto;

    public Animator animator;

    public GameObject pointerObject;

    private Input input;
    private NavMeshAgent agent;

    [Header("Settings")]
    public float moveDuration = 1.0f;
    //public float pointerBlinkTime = 1f;

    public bool beaverInArea = false;
    public bool isMoving = false;

    public bool inAvto = false;


    //private Coroutine blinkCoroutine;


    private void Awake()
    {
        input = new Input();

        input.player.EnterCar.performed += moveToPoint;

        agent = Beaver.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.enabled = false;
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


            pointerObject.SetActive(true);
            // if (blinkCoroutine == null)
            // {
            //     blinkCoroutine = StartCoroutine(BlinkDoorShine());
            // }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Із SenseArea вийшов: " + other.name);

        if (other.transform.root == Beaver.root)
        {
            beaverInArea = false;
            Debug.Log("БОБЕР ВИЙШОВ ІЗ SENSE AREA");

            pointerObject.SetActive(false);

            // if (blinkCoroutine != null)
            // {
            //     StopCoroutine(blinkCoroutine);
            //     blinkCoroutine = null;
            //     pointerObject.SetActive(false);
            // }
        }


    }

    private IEnumerator MoveBeaverToEnterPoint()
    {
        Debug.Log("Sit Animation, isMoving = " + isMoving + " inAvto = " + inAvto + " beaverInArea " + beaverInArea);

        isMoving = true;
        inAvto = false;

        Beaver.GetComponent<Moving>().enabled = false;

        pointerObject.SetActive(false);

        agent.enabled = true;
        agent.isStopped = false;
        agent.SetDestination(EnterPoint.position);

        animator.SetBool("IsGo", true);

        while (agent.pathPending)
        {
            yield return null;
        }

        while (agent.remainingDistance > agent.stoppingDistance)
        {
            Vector3 direction = agent.steeringTarget - Beaver.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);
                Beaver.rotation = Quaternion.Slerp(Beaver.rotation, targetRotation, 10f * Time.deltaTime);
            }

            yield return null;
        }

        agent.isStopped = true;

        animator.SetBool("IsGo", false);

        Beaver.position = EnterPoint.position;

        while (Quaternion.Angle(Beaver.rotation, EnterPoint.rotation) > 0.5f)
        {
            Beaver.position = EnterPoint.position;
            Beaver.rotation = Quaternion.Slerp(Beaver.rotation, EnterPoint.rotation, 5f * Time.deltaTime);
            yield return null;
        }

        Beaver.position = EnterPoint.position;
        Beaver.rotation = EnterPoint.rotation;

        animator.SetBool("IsOutOfCar", false);
        animator.SetBool("IsSitToCar", true);

        agent.enabled = false;

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

        inAvto = true;

        yield return new WaitForSeconds(2f);

        Beaver.SetParent(avto.transform, true);

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

    // private IEnumerator BlinkDoorShine()
    // {
    //     while (true)
    //     {
    //         pointerObject.SetActive(true);
    //         yield return new WaitForSeconds(pointerBlinkTime);

    //         pointerObject.SetActive(false);
    //         yield return new WaitForSeconds(pointerBlinkTime);
    //     }
    // }
}