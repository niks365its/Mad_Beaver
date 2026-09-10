using UnityEngine;
using UnityEngine.InputSystem;

public class MovingCar : MonoBehaviour
{
    private Input input;

    [Header("Objects")]
    public Transform FrontLeftWheel;
    public Transform FrontRightWheel;
    public Transform RearWheel;


    public WheelCollider FrontLeftWheelCollider;
    public WheelCollider FrontRightWheelCollider;
    public WheelCollider RearLeftWheelCollider;
    public WheelCollider RearRightWheelCollider;

    [Header("Motor settings")]
    public float motorTorque = 1500f;
    public float maxSteerAngle = 30f;
    public bool frontWheelDrive = true;
    public bool rearWheelDrive = false;
    public float massCenter = 0f;

    [Header("Brake settings")]
    public float brakeTorque = 3000f;
    public float idleBrakeTorque = 100f;

    private Vector2 CarMove;
    private bool isBraking = false;
    private bool isBrakingPressed = false;
    private float forwardSpeed;

    private Rigidbody rb;

    [Header("Mud and Smoke settings")]
    // public ParticleSystem FrontLeftMud;
    // public ParticleSystem FrontRightMud;
    public ParticleSystem RearLeftMud;
    public ParticleSystem RearRightMud;
    public ParticleSystem Smoke;

    public float mudMinSpeed = 1f;
    public float mudMaxSpeed = 10f;
    public float mudMinSlip = 0.2f;
    public float mudMaxSlip = 1.5f;

    public float smokeMinSlip = 0.2f;
    public float smokeMaxSlip = 1.5f;

    void Awake()
    {
        input = new Input();
        input.player.CarMove.performed += moveCar;
        input.player.CarMove.canceled += moveCar;
        input.player.Brake.performed += brakeCar;
        input.player.Brake.canceled += brakeCar;

        rb = GetComponent<Rigidbody>();
        // rb.centerOfMass = new Vector3(0f, massCenter, 0f);
    }

    void OnEnable()
    {
        input.Enable();
        LightControl.Instance.SetFrontLight(true);

        if (Smoke != null)
            Smoke.Play();
    }

    void OnDisable()
    {
        input.Disable();
        LightControl.Instance.SetFrontLight(false);
        Smoke.Stop();
        RearLeftMud.Stop();
        RearRightMud.Stop();
    }

    void FixedUpdate()
    {
        float motor = 0f;
        float input = -CarMove.y;
        forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        bool isReverseBrake = Mathf.Abs(forwardSpeed) > 0.1f && Mathf.Sign(input) != Mathf.Sign(forwardSpeed) && input != 0f;

        if (isReverseBrake)
        {
            motor = 0f;

        }
        else
        {
            float currentTorque = motorTorque;

            if (input > 0f)
            {
                currentTorque /= 4f;
            }
            Debug.Log("Input is " + input + " Power is " + currentTorque);
            motor = input * currentTorque;

        }

        isBraking = isBrakingPressed || isReverseBrake;

        ApplyBrakes();

        if (frontWheelDrive && rearWheelDrive)
        {
            float doubleTorque = motorTorque * 0.5f;
            FrontLeftWheelCollider.motorTorque = doubleTorque;
            FrontRightWheelCollider.motorTorque = doubleTorque;
            RearLeftWheelCollider.motorTorque = doubleTorque;
            RearRightWheelCollider.motorTorque = doubleTorque;
        }

        else if (frontWheelDrive)
        {
            FrontLeftWheelCollider.motorTorque = motor;
            FrontRightWheelCollider.motorTorque = motor;
        }
        else if (rearWheelDrive)
        {
            RearLeftWheelCollider.motorTorque = motor;
            RearRightWheelCollider.motorTorque = motor;
        }

        float steer = CarMove.x * maxSteerAngle;

        FrontLeftWheelCollider.steerAngle = steer;
        FrontRightWheelCollider.steerAngle = steer;

        rb.centerOfMass = new Vector3(0f, massCenter, 0f);

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

        UpdateMudParticles();
        UpdateSmokeParticles();
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


        if (isBraking)
        {
            // Debug.Log("Brake = " + brake + " | FL = " + FrontLeftWheelCollider.brakeTorque + "Speed " + Mathf.Abs(forwardSpeed));
            brake = brakeTorque * Mathf.Abs(forwardSpeed);
            LightControl.Instance.SetBrakeLight(true);
        }
        else if (Mathf.Abs(CarMove.y) < 0.01f)
        {
            brake = idleBrakeTorque * Mathf.Abs(forwardSpeed);
            LightControl.Instance.SetBrakeLight(false);
        }
        else
        {
            LightControl.Instance.SetBrakeLight(false);
        }
        //   Debug.Log("FL Brake = " + brake);
        FrontLeftWheelCollider.brakeTorque = brake;
        FrontRightWheelCollider.brakeTorque = brake;
        RearLeftWheelCollider.brakeTorque = brake;
        RearRightWheelCollider.brakeTorque = brake;
    }

    private void brakeCar(InputAction.CallbackContext context)
    {
        isBrakingPressed = context.ReadValueAsButton();

        // Debug.Log("Is braking = " + isBraking);
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

    void UpdateMudParticles()
    {
        // UpdateWheelMud(FrontLeftWheelCollider, FrontLeftMud);
        // UpdateWheelMud(FrontRightWheelCollider, FrontRightMud);
        UpdateWheelMud(RearLeftWheelCollider, RearLeftMud);
        UpdateWheelMud(RearRightWheelCollider, RearRightMud);

    }

    void UpdateWheelMud(WheelCollider wheel, ParticleSystem mud)
    {
        if (mud == null)
            return;

        if (!wheel.GetGroundHit(out WheelHit hit))
        {
            if (mud.isPlaying)
                mud.Stop();

            return;
        }

        float speed = Mathf.Abs(forwardSpeed);

        if (speed < mudMinSpeed)
        {
            if (mud.isPlaying)
                mud.Stop();

            return;
        }

        float slip = Mathf.Max(Mathf.Abs(hit.forwardSlip), Mathf.Abs(hit.sidewaysSlip));

        float speedAmount = Mathf.InverseLerp(mudMinSpeed, mudMaxSpeed, speed);
        float slipAmount = Mathf.InverseLerp(mudMinSlip, mudMaxSlip, slip);

        float amount = Mathf.Max(speedAmount, slipAmount);

        var emission = mud.emission;
        emission.rateOverTime = Mathf.Lerp(2f, 25f, amount);

        if (!mud.isPlaying)
            mud.Play();
    }

    private void UpdateSmokeParticles()
    {
        UpdateSmoke(RearRightWheelCollider, Smoke);
    }

    void UpdateSmoke(WheelCollider wheel, ParticleSystem smoke)
    {
        if (smoke == null)
        {
            return;
        }

        wheel.GetGroundHit(out WheelHit hit);

        float slip = Mathf.Max(Mathf.Abs(hit.forwardSlip), Mathf.Abs(hit.sidewaysSlip));
        float amount = Mathf.InverseLerp(smokeMinSlip, smokeMaxSlip, slip);

        var emission = smoke.emission;
        emission.rateOverTime = Mathf.Lerp(70f, 200f, amount);

        var main = smoke.main;
        main.simulationSpeed = Mathf.Lerp(0.2f, 1f, amount);
        main.startLifetime = Mathf.Lerp(0.2f, 0.5f, amount);
    }
}