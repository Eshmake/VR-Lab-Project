using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DryingTowel : MonoBehaviour
{
    [Header("Drying Motion")]
    [Tooltip("Minimum towel speed (m/s) to start counting drying time.")]
    public float minSpeedForDrying = 0.2f;
    [Tooltip("Speed (m/s) that maps to full intensity; higher speeds give no extra benefit.")]
    public float maxSpeedForFullIntensity = 1.0f;

    [Header("Filters")]
    [Tooltip("Only affect objects with this tag (leave blank to ignore).")]
    public string sampleTag = "Sample";

    private Collider _trigger;
    private Vector3 _lastPos;

    void Awake()
    {
        _trigger = GetComponent<Collider>();
        _trigger.isTrigger = true; // IMPORTANT
        _lastPos = transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        // Find the RockSample on the overlapped object
        var sample = other.GetComponentInParent<RockSample>();
        if (sample == null) return;

        if (!string.IsNullOrEmpty(sampleTag) && !other.CompareTag(sampleTag) && !sample.CompareTag(sampleTag))
            return;

        // Estimate towel speed
        float speed = (transform.position - _lastPos).magnitude / Mathf.Max(Time.deltaTime, 1e-5f);
        if (speed < minSpeedForDrying) { _lastPos = transform.position; return; }

        // Map speed -> intensity 0..1 (optional headroom)
        float intensity01 = Mathf.InverseLerp(minSpeedForDrying, maxSpeedForFullIntensity, speed);

        // Advance drying time
        sample.DryStep(Time.deltaTime * Mathf.Clamp01(intensity01));

        _lastPos = transform.position;
    }

    void LateUpdate()
    {
        // Keep lastPos tracking even when not overlapping
        _lastPos = transform.position;
    }
}
