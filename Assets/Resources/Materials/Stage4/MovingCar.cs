using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MovingCar : MonoBehaviour
{
    private Input input;
    public GameObject Avto;

    public Transform FrontLeftPivot;
    public Transform FrontRightPivot;

    public Transform FrontLeftWheel;
    public Transform FrontRightWheel;
    public Transform RearWheel;

    public float wheelRadius = 0.35f;

    public float maxSteerAngle = 30f;
    public float speed = 5f;
    public float rotationSpeed = 100f;
    private Vector2 CarMove;

    public Animator animator;

    private bool isMove = false;



    void Awake()

    {
        input = new Input();

        input.player.CarMove.performed += moveCar;
        input.player.CarMove.canceled += moveCar;

    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        Debug.Log(CarMove);
        Avto.transform.Translate(Vector3.forward * -CarMove.y * speed * Time.deltaTime);

        if (CarMove.y != 0)
        {
            Avto.transform.Rotate(Vector3.up * CarMove.x * rotationSpeed * Time.deltaTime);
        }

        float steer = CarMove.x * maxSteerAngle;

        if (CarMove.y < 0)
        {
            steer = -steer;
        }

        FrontLeftPivot.localRotation =
            Quaternion.Euler(0f, steer, 0f);

        FrontRightPivot.localRotation =
            Quaternion.Euler(0f, steer, 0f);

        float distance = CarMove.y * speed * Time.deltaTime;
        float wheelAngle = (distance / (2f * Mathf.PI * wheelRadius)) * 360f;

        FrontLeftWheel.Rotate(Vector3.right, wheelAngle);
        FrontRightWheel.Rotate(Vector3.right, wheelAngle);
        RearWheel.Rotate(Vector3.right, wheelAngle);
    }

    private void moveCar(InputAction.CallbackContext context)
    {
        CarMove = context.ReadValue<Vector2>();
        isMove = CarMove != Vector2.zero;



        animator.SetBool("IsGo", isMove && CarMove.y > 0);

        // Рух назад → Mirror
        animator.SetBool("IsGoBack", isMove && CarMove.y < 0);
    }

}
