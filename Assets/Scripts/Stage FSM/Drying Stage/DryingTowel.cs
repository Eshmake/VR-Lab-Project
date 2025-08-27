using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DryingTowel : MonoBehaviour
{
    [Header("Drying Trigger")]
    [Tooltip("Trigger collider used to detect overlap with rocks. Recommended to be a separate collider.")]
    public Collider towelTrigger;   // assign a Trigger collider here

    [Header("Drying Motion")]
    [Tooltip("Minimum towel speed (m/s) to begin drying.")]
    public float minSpeedForDrying = 0.2f;
    [Tooltip("Towel speed (m/s) that maps to full intensity.")]
    public float maxSpeedForFullIntensity = 1.0f;

    [Header("Filters")]
    [Tooltip("Only affect objects with this tag (leave blank to ignore).")]
    public string sampleTag = "Sample";

    [Header("Audio (optional)")]
    [Tooltip("Looping source that plays only while rubbing is happening.")]
    public AudioSource rubLoopSource;
    [Tooltip("Seconds to keep the loop alive after rubbing stops (prevents stutter).")]
    public float stopDelay = 0.15f;

    // --- runtime ---
    Vector3 _lastPos;
    float _rubHoldTimer = 0f;

    // what we’re overlapping right now
    readonly HashSet<RockSample> _overlappingSamples = new HashSet<RockSample>();

    void Awake()
    {
        if (towelTrigger == null)
        {
            towelTrigger = GetComponent<Collider>();
        }
        if (towelTrigger == null)
        {
            Debug.LogError("[DryingTowel] No collider assigned/found. Please assign a Trigger collider.");
            enabled = false;
            return;
        }
        if (!towelTrigger.isTrigger)
        {
            Debug.LogWarning("[DryingTowel] Assigned towelTrigger is not set as Trigger. Set isTrigger = true to detect overlaps.");
        }

        _lastPos = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        var sample = GetSample(other);
        if (sample != null)
            _overlappingSamples.Add(sample);
    }

    void OnTriggerExit(Collider other)
    {
        var sample = GetSample(other);
        if (sample != null)
            _overlappingSamples.Remove(sample);

        // if nothing left overlapping, start stop timer for loop
        if (_overlappingSamples.Count == 0)
            _rubHoldTimer = stopDelay;
    }

    void OnTriggerStay(Collider other)
    {
        // Estimate towel speed no matter what we hit (we’ll gate by wetness below)
        float speed = (transform.position - _lastPos).magnitude / Mathf.Max(Time.deltaTime, 1e-5f);

        // Is there at least one overlapped sample that is NOT dry?
        bool anyWet = false;
        foreach (var s in _overlappingSamples)
        {
            if (s != null && !s.IsDry) { anyWet = true; break; }
        }

        // You’re “rubbing” only if moving fast enough AND still overlapping a wet rock
        bool rubbingNow = speed >= minSpeedForDrying && anyWet;

        // Drive loop SFX
        HandleRubLoop(rubbingNow);

        // Apply drying to the specific sample we’re staying with (if it’s wet)
        if (rubbingNow)
        {
            var sample = GetSample(other);
            if (sample != null && !sample.IsDry)
            {
                float t = Mathf.InverseLerp(minSpeedForDrying, maxSpeedForFullIntensity, speed);
                sample.DryStep(Time.deltaTime * Mathf.Clamp01(t));
            }
        }

        _lastPos = transform.position;
    }

    void LateUpdate()
    {
        _lastPos = transform.position;

        // If we’re not overlapping anything wet, count down toward stopping loop
        if (_overlappingSamples.Count == 0 && rubLoopSource != null && rubLoopSource.isPlaying)
        {
            if (_rubHoldTimer > 0f)
            {
                _rubHoldTimer -= Time.deltaTime;
                if (_rubHoldTimer <= 0f) rubLoopSource.Stop();
            }
        }
    }

    RockSample GetSample(Collider other)
    {
        // tag filter (optional)
        if (!string.IsNullOrEmpty(sampleTag))
        {
            if (!other.CompareTag(sampleTag) && !other.transform.root.CompareTag(sampleTag))
                return null;
        }
        return other.GetComponentInParent<RockSample>();
    }

    void HandleRubLoop(bool rubbingNow)
    {
        if (rubLoopSource == null) return;

        if (rubbingNow)
        {
            _rubHoldTimer = 0f;
            if (!rubLoopSource.isPlaying)
            {
                rubLoopSource.loop = true;
                rubLoopSource.Play();
            }
        }
        else
        {
            // If not rubbing, start/continue the grace period
            if (_overlappingSamples.Count == 0)
            {
                if (rubLoopSource.isPlaying && _rubHoldTimer <= 0f)
                    _rubHoldTimer = stopDelay;
            }
            else
            {
                // Overlapping but not rubbing (too slow or all dry) → stop after delay
                if (rubLoopSource.isPlaying)
                {
                    if (_rubHoldTimer <= 0f) _rubHoldTimer = stopDelay;
                    _rubHoldTimer -= Time.deltaTime;
                    if (_rubHoldTimer <= 0f) rubLoopSource.Stop();
                }
            }
        }
    }
}

