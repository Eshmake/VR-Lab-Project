using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HorizontalShakeTask : MonoBehaviour
{
    [Header("Refs")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;        // assign on the sieve
    public Rigidbody rb;                   // optional; if null we use transform deltas

    [Header("Shake Completion Logic")]
    public float minHorizontalSpeed = 0.6f;
    public float minFlipAngle = 100f;
    public float requiredShakeTime = 2.5f;
    public bool onlyWhenHeld = true;
    public Vector3 planeNormal = Vector3.up;

    [Header("Realtime Shake State (for audio/FX)")]
    [Tooltip("Speed above which we consider 'shaking has started'.")]
    public float shakeStartSpeed = 0.35f;
    [Tooltip("Speed below which we consider 'shaking has stopped'. (Hysteresis)")]
    public float shakeStopSpeed = 0.20f;
    [Tooltip("Smoothing factor (0..1) for displayed speed; higher = snappier.")]
    [Range(0f, 1f)] public float speedSmoothing = 0.35f;

    [Header("Events (Optional)")]
    public UnityEvent onShakeStarted;
    public UnityEvent onShakeStopped;
    public UnityEvent onShakeCompleted;

    [Header("Debug")]
    public bool isShakenComplete;
    public float accumulatedShakeTime;
    public float CurrentPlanarSpeed;   // <-- expose for audio logic
    public bool IsCurrentlyShaking;    // <-- expose for audio logic

    // internal
    Vector3 _prevPos;
    Vector3 _prevVelOnPlane;
    bool _wasHeld;

    void Reset()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (grab == null) grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnReleased);
        }

        _prevPos = transform.position;
        _prevVelOnPlane = Vector3.zero;
        accumulatedShakeTime = 0f;
        isShakenComplete = false;
        SetShaking(false, invokeEvent: false); // reset state
        CurrentPlanarSpeed = 0f;
    }

    void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }
    }

    void OnGrabbed(SelectEnterEventArgs _) { _wasHeld = true; }
    void OnReleased(SelectExitEventArgs _) { _wasHeld = false; }

    void Update()
    {
        if (isShakenComplete)
        { // still update speed/state for audio if you like; or early-return
            UpdateSpeedAndState();
            return;
        }

        if (onlyWhenHeld && !_wasHeld)
        {
            // reset speed while not held to prevent stale values
            CurrentPlanarSpeed = Mathf.Lerp(CurrentPlanarSpeed, 0f, 0.5f);
            _prevPos = transform.position;
            SetShaking(false);
            return;
        }

        // Update instantaneous speed & state (for audio feedback)
        UpdateSpeedAndState();

        // Core “task completion” logic (accumulate shake time)
        if (CurrentPlanarSpeed >= minHorizontalSpeed)
        {
            // also require direction flipping for extra rigor
            float flipAngle = Vector3.Angle(_prevVelOnPlane, GetPlanarVelocity());
            bool flippedDirection = flipAngle >= minFlipAngle;

            float delta = Time.deltaTime * (flippedDirection ? 1.25f : 1.0f);
            accumulatedShakeTime += delta;

            if (accumulatedShakeTime >= requiredShakeTime)
            {
                isShakenComplete = true;
                onShakeCompleted?.Invoke();
            }
        }

        // remember previous planar velocity (low-pass)
        _prevVelOnPlane = Vector3.Lerp(_prevVelOnPlane, GetPlanarVelocity(), 0.5f);
    }

    void UpdateSpeedAndState()
    {
        Vector3 planarVel = GetPlanarVelocity();
        float rawSpeed = planarVel.magnitude;

        // smooth the exposed speed for stable audio gating
        CurrentPlanarSpeed = Mathf.Lerp(CurrentPlanarSpeed, rawSpeed, Mathf.Clamp01(speedSmoothing));

        // hysteresis: start above start threshold, stop below stop threshold
        if (!IsCurrentlyShaking && CurrentPlanarSpeed >= shakeStartSpeed)
            SetShaking(true);
        else if (IsCurrentlyShaking && CurrentPlanarSpeed <= shakeStopSpeed)
            SetShaking(false);
    }

    Vector3 GetPlanarVelocity()
    {
        Vector3 worldVel = rb != null
            ? rb.linearVelocity
            : (transform.position - _prevPos) / Mathf.Max(Time.deltaTime, 0.0001f);

        _prevPos = transform.position;

        Vector3 n = planeNormal.sqrMagnitude > 1e-6f ? planeNormal.normalized : Vector3.up;
        return Vector3.ProjectOnPlane(worldVel, n);
    }

    void SetShaking(bool value, bool invokeEvent = true)
    {
        if (IsCurrentlyShaking == value) return;
        IsCurrentlyShaking = value;

        if (!invokeEvent) return;

        if (value) onShakeStarted?.Invoke();
        else onShakeStopped?.Invoke();
    }

    public void ResetTask()
    {
        accumulatedShakeTime = 0f;
        isShakenComplete = false;
        SetShaking(false, invokeEvent: false);
        CurrentPlanarSpeed = 0f;
    }
}

