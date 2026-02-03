using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class HeldAntiPhasing : MonoBehaviour
{
    [Header("Collision")]
    [Tooltip("A NON-trigger collider on the towel used for blocking. (Not the drying trigger).")]
    public Collider solidCollider;

    [Tooltip("Layers that should block the towel (cabinet, walls, etc.). Exclude Player/Hands.")]
    public LayerMask environmentMask = ~0;

    [Tooltip("How many push-out iterations per frame while held (more = stronger, costlier).")]
    [Range(1, 8)] public int iterations = 4;

    [Tooltip("Max push distance per iteration (meters).")]
    public float maxPushPerIteration = 0.03f;

    [Header("Fail-safe")]
    [Tooltip("If penetration exceeds this (meters) in any single contact, snap back to last safe pose.")]
    public float snapBackIfPenetrationOver = 0.10f;

    [Tooltip("How many frames in a row we allow invalid overlap before snapping back.")]
    public int graceFrames = 2;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    bool _held;

    Transform _attach;                 // where the hand wants it
    Vector3 _lastSafePos;
    Quaternion _lastSafeRot;

    int _badFrames;

    readonly Collider[] _hits = new Collider[48];

    void Awake()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (solidCollider == null)
            solidCollider = GetComponent<Collider>();

        _grab.selectEntered.AddListener(OnGrab);
        _grab.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        _held = true;
        _attach = args.interactorObject.transform;

        _lastSafePos = transform.position;
        _lastSafeRot = transform.rotation;
        _badFrames = 0;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        _held = false;
        _attach = null;
        _badFrames = 0;
    }

    void LateUpdate()
    {
        if (!_held || solidCollider == null) return;

        bool validThisFrame = ResolvePenetrations(out float worstPenetration);

        if (validThisFrame)
        {
            _lastSafePos = transform.position;
            _lastSafeRot = transform.rotation;
            _badFrames = 0;
        }
        else
        {
            _badFrames++;

            // If we�re deeply inside or it stays bad for a couple frames, snap back.
            if (worstPenetration >= snapBackIfPenetrationOver || _badFrames >= graceFrames)
            {
                transform.SetPositionAndRotation(_lastSafePos, _lastSafeRot);

                // As a second safety: if lastSafe is also bad due to extreme motion, snap to hand.
                // (This prevents disappearing under cabinets forever.)
                ResolvePenetrations(out _);

                _badFrames = 0;
            }
        }
    }

    bool ResolvePenetrations(out float worstPenetration)
    {
        worstPenetration = 0f;
        bool anyOverlap = false;

        // Run a few iterations so even deep/fast intersection gets corrected quickly.
        for (int it = 0; it < iterations; it++)
        {
            Bounds b = solidCollider.bounds;

            int count = Physics.OverlapBoxNonAlloc(
                b.center,
                b.extents,
                _hits,
                solidCollider.transform.rotation,
                environmentMask,
                QueryTriggerInteraction.Ignore
            );

            Vector3 totalPush = Vector3.zero;
            int pushes = 0;

            for (int i = 0; i < count; i++)
            {
                var other = _hits[i];
                _hits[i] = null;
                if (other == null) continue;
                if (other.transform.IsChildOf(transform)) continue;

                if (Physics.ComputePenetration(
                        solidCollider, solidCollider.transform.position, solidCollider.transform.rotation,
                        other, other.transform.position, other.transform.rotation,
                        out Vector3 dir, out float dist))
                {
                    if (dist > 0f)
                    {
                        anyOverlap = true;
                        worstPenetration = Mathf.Max(worstPenetration, dist);

                        totalPush += dir * dist;
                        pushes++;
                    }
                }
            }

            if (pushes == 0)
                break;

            // Clamp push so we don�t jump.
            Vector3 push = totalPush;
            float mag = push.magnitude;
            if (mag > maxPushPerIteration)
                push = push * (maxPushPerIteration / mag);

            transform.position += push;
        }

        // Valid if we ended with no overlaps (or very tiny overlaps)
        // If you want stricter, treat anyOverlap as invalid.
        return !anyOverlap || worstPenetration < 0.001f;
    }
}
