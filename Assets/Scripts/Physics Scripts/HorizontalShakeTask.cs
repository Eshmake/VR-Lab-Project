using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HorizontalShakeTask : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    public Rigidbody rb;

    public float minHorizontalSpeed = 0.6f;
    public float minFlipAngle = 100f;
    public float requiredShakeTime = 2.5f;
    public bool onlyWhenHeld = true;
    public Vector3 planeNormal = Vector3.up;

    public bool isShakenComplete;
    public float accumulatedShakeTime;

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

        // ✅ UnityEvent-style subscription
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnReleased);
        }

        _prevPos = transform.position;
        _prevVelOnPlane = Vector3.zero;
        accumulatedShakeTime = 0f;
        isShakenComplete = false;
    }

    void OnDisable()
    {
        if (grab != null)
        {
            // ✅ UnityEvent-style unsubscription
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }
    }

    void OnGrabbed(SelectEnterEventArgs _) { _wasHeld = true; }
    void OnReleased(SelectExitEventArgs _) { _wasHeld = false; }

    void Update()
    {
        if (isShakenComplete) return;
        if (onlyWhenHeld && !_wasHeld) { _prevPos = transform.position; return; }

        Vector3 worldVel = rb != null
            ? rb.linearVelocity
            : (transform.position - _prevPos) / Mathf.Max(Time.deltaTime, 0.0001f);

        _prevPos = transform.position;

        Vector3 n = planeNormal.sqrMagnitude > 0.0001f ? planeNormal.normalized : Vector3.up;
        Vector3 velOnPlane = Vector3.ProjectOnPlane(worldVel, n);

        float speed = velOnPlane.magnitude;
        bool fastEnough = speed >= minHorizontalSpeed;

        float flipAngle = Vector3.Angle(_prevVelOnPlane, velOnPlane);
        bool flippedDirection = flipAngle >= minFlipAngle;

        if (fastEnough)
        {
            float delta = Time.deltaTime * (flippedDirection ? 1.25f : 1.0f);
            accumulatedShakeTime += delta;

            if (accumulatedShakeTime >= requiredShakeTime)
                isShakenComplete = true;
        }

        _prevVelOnPlane = Vector3.Lerp(_prevVelOnPlane, velOnPlane, 0.5f);
    }

    public void ResetTask()
    {
        accumulatedShakeTime = 0f;
        isShakenComplete = false;
    }
}

