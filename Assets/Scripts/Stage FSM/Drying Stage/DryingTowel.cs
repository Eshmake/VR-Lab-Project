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

    Vector3 _lastPos;

    void Awake()
    {
        if (towelTrigger == null)
        {
            // fallback to a collider on this object
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

    void OnTriggerStay(Collider other)
    {
        // Only process when the overlap comes from the assigned trigger
        if (towelTrigger == null || !ReferenceEquals(other, other)) { /* placeholder to keep method */ }

        // We can’t filter which collider invoked OnTriggerStay directly,
        // so ensure ONLY the towelTrigger has 'isTrigger = true'.
        // Any physics colliders on this rigidbody should be non-trigger.

        var sample = other.GetComponentInParent<RockSample>();
        if (sample == null) return;

        if (!string.IsNullOrEmpty(sampleTag))
        {
            if (!other.CompareTag(sampleTag) && !sample.gameObject.CompareTag(sampleTag))
                return;
        }

        float speed = (transform.position - _lastPos).magnitude / Mathf.Max(Time.deltaTime, 1e-5f);
        if (speed < minSpeedForDrying) { _lastPos = transform.position; return; }

        float t = Mathf.InverseLerp(minSpeedForDrying, maxSpeedForFullIntensity, speed);
        sample.DryStep(Time.deltaTime * Mathf.Clamp01(t));

        _lastPos = transform.position;
    }

    void LateUpdate()
    {
        _lastPos = transform.position;
    }
}
