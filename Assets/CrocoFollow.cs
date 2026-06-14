using UnityEngine;

public class CrocoFollow : MonoBehaviour
{
    public Transform target;
    public Transform hunter;

    [Header("Speed")]
    public float maxSpeed = 3f;          // max units per FixedUpdate frame

    [Header("Catch")]
    public float catchDistance = 0.2f;   // X-axis distance considered "caught"

    // tracks whether hunter has reached the target
    private bool _caught = false;

    void FixedUpdate()
    {
        if (target == null || hunter == null) return;

        Vector3 hPos = hunter.position;
        Vector3 tPos = target.position;

        float deltaX = tPos.x - hPos.x;
        float absDX = Mathf.Abs(deltaX);

        // ── state switch ────────────────────────────────────────────
        if (_caught && absDX > catchDistance)
            _caught = false;
        else if (!_caught && absDX <= catchDistance)
            _caught = true;

        // ── movement ────────────────────────────────────────────────
        if (!_caught)
        {
            // Chase: move toward target on both axes, capped at maxSpeed
            float moveX = Mathf.Clamp(deltaX, -maxSpeed, maxSpeed);
            float moveY = Mathf.Clamp(tPos.y - hPos.y, -maxSpeed, maxSpeed);

            hPos.x += moveX;
            hPos.y += moveY;
        }
        else
        {
            // Caught: figure out how fast the target moved this frame on X,
            // copy that speed (capped), and snap Y to target.
            float targetSpeedX = (tPos.x - _prevTargetX);   // delta since last frame
            float clampedSpeedX = Mathf.Clamp(targetSpeedX, -maxSpeed, maxSpeed);

            hPos.x += clampedSpeedX;
            hPos.y = tPos.y;           // lock Y exactly to target
        }

        hunter.position = hPos;

        // remember target X for next frame speed calculation
        _prevTargetX = tPos.x;
    }

    // ── internals ───────────────────────────────────────────────────
    private float _prevTargetX;

    void Awake()
    {
        // seed so first caught-frame speed reads as 0
        if (target != null)
            _prevTargetX = target.position.x;
    }
}