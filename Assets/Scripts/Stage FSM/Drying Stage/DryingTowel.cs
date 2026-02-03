using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DryingTowel : MonoBehaviour
{
    [Header("Drying Trigger")]
    [Tooltip("Trigger collider used to detect overlap with samples. Recommended to be a separate collider.")]
    public Collider towelTrigger;

    [Header("Drying Speed (Independent of movement)")]
    [Tooltip("How many seconds of contact it should take to fully dry a sample (smaller = faster).")]
    public float secondsToDry = 1.0f;

    [Tooltip("Dry multiple overlapping samples at the same time.")]
    public bool dryAllOverlappingSamples = true;

    [Header("Filters")]
    [Tooltip("Only affect objects with this tag (leave blank to ignore).")]
    public string sampleTag = "Sample";

    [Tooltip("Optional layer mask to restrict what the trigger checks.")]
    public LayerMask overlapMask = ~0;

    [Header("Audio (optional)")]
    [Tooltip("Looping source that plays only while touching wet samples.")]
    public AudioSource rubLoopSource;

    [Tooltip("Seconds to keep the loop alive after contact stops (prevents stutter).")]
    public float stopDelay = 0.15f;

    // ---- runtime ----
    readonly Collider[] _overlapResults = new Collider[64];
    readonly HashSet<RockSample> _overlappingSamples = new HashSet<RockSample>();
    float _stopTimer;

    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        // Trigger events/overlaps are most reliable when *something* has a Rigidbody.
        // In VR this is often grabbed/moved, so keep it kinematic unless you need physics.
        if (_rb != null)
        {
           // _rb.isKinematic = true;
           _rb.useGravity = true;
        }

        if (towelTrigger == null)
            towelTrigger = GetComponent<Collider>();

        if (towelTrigger == null)
        {
            Debug.LogError("[DryingTowel] No collider assigned/found. Please assign a Trigger collider.");
            enabled = false;
            return;
        }

        if (!towelTrigger.isTrigger)
            Debug.LogWarning("[DryingTowel] towelTrigger is not set as Trigger. Set isTrigger = true.");
    }

    void Update()
    {
        // 1) Refresh overlaps robustly every frame (no dependence on OnTriggerEnter/Exit/Stay)
        RefreshOverlaps();

        // 2) Dry anything wet we’re touching, at a constant rate (independent of movement speed)
        bool touchingAnyWet = DryOverlaps();

        // 3) Audio
        HandleLoop(touchingAnyWet);
    }

    void RefreshOverlaps()
    {
        _overlappingSamples.Clear();

        // Use ClosestPoint-style overlap via physics query around the trigger bounds.
        // This is very stable even if objects are enabled/disabled/teleported.
        Bounds b = towelTrigger.bounds;
        Vector3 center = b.center;
        Vector3 halfExtents = b.extents;

        int count = Physics.OverlapBoxNonAlloc(
            center,
            halfExtents,
            _overlapResults,
            towelTrigger.transform.rotation,
            overlapMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < count; i++)
        {
            var col = _overlapResults[i];
            if (col == null) continue;

            var sample = GetSample(col);
            if (sample != null)
                _overlappingSamples.Add(sample);

            _overlapResults[i] = null; // clear slot
        }
    }

    bool DryOverlaps()
    {
        if (_overlappingSamples.Count == 0) return false;

        float dryStepPerSecond = (secondsToDry <= 0.0001f) ? 1f : (1f / secondsToDry);
        float step = dryStepPerSecond * Time.deltaTime;

        bool anyWet = false;

        if (dryAllOverlappingSamples)
        {
            foreach (var s in _overlappingSamples)
            {
                if (s == null) continue;
                if (s.IsDry) continue;

                anyWet = true;
                s.DryStep(step);
            }
        }
        else
        {
            // Dry just one (first wet) sample if you prefer that behavior
            foreach (var s in _overlappingSamples)
            {
                if (s == null) continue;
                if (s.IsDry) continue;

                anyWet = true;
                s.DryStep(step);
                break;
            }
        }

        return anyWet;
    }

    RockSample GetSample(Collider other)
    {
        if (other == null) return null;

        // Tag filter (optional)
        if (!string.IsNullOrEmpty(sampleTag))
        {
            bool ok = other.CompareTag(sampleTag) || other.transform.root.CompareTag(sampleTag);
            if (!ok) return null;
        }

        return other.GetComponentInParent<RockSample>();
    }

    void HandleLoop(bool touchingWet)
    {
        if (rubLoopSource == null) return;

        if (touchingWet)
        {
            _stopTimer = stopDelay;

            if (!rubLoopSource.isPlaying)
            {
                rubLoopSource.loop = true;
                rubLoopSource.Play();
            }
        }
        else
        {
            if (rubLoopSource.isPlaying)
            {
                _stopTimer -= Time.deltaTime;
                if (_stopTimer <= 0f)
                    rubLoopSource.Stop();
            }
        }
    }

#if UNITY_EDITOR
    // Helpful gizmo to see overlap volume (uses towelTrigger bounds)
    void OnDrawGizmosSelected()
    {
        if (towelTrigger == null) return;
        Gizmos.matrix = Matrix4x4.TRS(towelTrigger.bounds.center, towelTrigger.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, towelTrigger.bounds.size);
    }
#endif
}
