using UnityEngine;
using UnityEngine.InputSystem;

public class MovingCar : MonoBehaviour
{
    private Input input;

    public Transform FrontLeftWheel;
    public Transform FrontRightWheel;
    public Transform RearWheel;


    public WheelCollider FrontLeftWheelCollider;
    public WheelCollider FrontRightWheelCollider;
    public WheelCollider RearLeftWheelCollider;
    public WheelCollider RearRightWheelCollider;

    public float motorTorque = 1500f;
    public float maxSteerAngle = 30f;

    [Header("Гальма")]
    public float brakeTorque = 3000f;
    public float idleBrakeTorque = 100f;

    public float massCenter = 0f;

    private Vector2 CarMove;
    private bool isBraking = false;

    private Rigidbody rb;

    void Awake()
    {
        input = new Input();
        input.player.CarMove.performed += moveCar;
        input.player.CarMove.canceled += moveCar;
        input.player.Brake.performed += brakeCar;
        input.player.Brake.canceled += brakeCar;

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, massCenter, 0f);
    }

    void OnEnable()
    {
        input.Enable();
        LightControl.Instance.SetFrontLight(true);
    }

    void OnDisable()
    {
        input.Disable();
        LightControl.Instance.SetFrontLight(false);
    }

    void FixedUpdate()
    {
        float motor = isBraking ? 0f : -CarMove.y * motorTorque;

        FrontLeftWheelCollider.motorTorque = motor;
        FrontRightWheelCollider.motorTorque = motor;

        float steer = CarMove.x * maxSteerAngle;

        FrontLeftWheelCollider.steerAngle = steer;
        FrontRightWheelCollider.steerAngle = steer;



        if (CarMove.y < -0.2f)
        {
            LightControl.Instance.SetRearGearLight(true);
        }
        else
        {
            LightControl.Instance.SetRearGearLight(false);
        }

        if (CarMove.x > 0.2f)
        {
            LightControl.Instance.StartRightBlink();
        }
        else if (CarMove.x < -0.2f)
        {
            LightControl.Instance.StartLeftBlink();
        }
        else
        {
            LightControl.Instance.StopRightBlink();
            LightControl.Instance.StopLeftBlink();
        }

        ApplyBrakes();
    }

    void LateUpdate()
    {
        UpdateWheelVisual(FrontLeftWheelCollider, FrontLeftWheel);
        UpdateWheelVisual(FrontRightWheelCollider, FrontRightWheel);
        UpdateWheelVisual(RearLeftWheelCollider, RearWheel);
    }

    void Update()
    {

    }

    void UpdateWheelVisual(WheelCollider collider, Transform wheel)
    {
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        if (wheel != RearWheel)
        {
            wheel.position = position;
        }
        wheel.rotation = rotation;
    }

    private void moveCar(InputAction.CallbackContext context)
    {
        CarMove = context.ReadValue<Vector2>();
    }

    void ApplyBrakes()
    {

        float brake = 0f;
        Debug.Log("Brake = " + brake + " | FL = " + FrontLeftWheelCollider.brakeTorque);

        if (isBraking)
        {
            brake = brakeTorque;
            LightControl.Instance.SetBrakeLight(true);
        }
        else if (Mathf.Abs(CarMove.y) < 0.01f)
        {
            brake = idleBrakeTorque;
            LightControl.Instance.SetBrakeLight(false);
        }
        else
        {
            LightControl.Instance.SetBrakeLight(false);
        }

        FrontLeftWheelCollider.brakeTorque = brake;
        FrontRightWheelCollider.brakeTorque = brake;
        RearLeftWheelCollider.brakeTorque = brake;
        RearRightWheelCollider.brakeTorque = brake;
    }

    private void brakeCar(InputAction.CallbackContext context)
    {
        isBraking = context.ReadValueAsButton();

        Debug.Log("Is braking = " + isBraking);
    }

    void OnDrawGizmos()
    {


        Rigidbody rigidbody = GetComponent<Rigidbody>();

        if (rigidbody == null)
            return;

        Vector3 centerOfMassWorld = transform.TransformPoint(rigidbody.centerOfMass);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(centerOfMassWorld, 0.1f);

        Gizmos.DrawLine(centerOfMassWorld, centerOfMassWorld + Vector3.up * 0.5f);
    }
}