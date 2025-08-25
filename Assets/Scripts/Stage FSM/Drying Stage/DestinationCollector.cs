using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RockSampleEvent : UnityEvent<RockSample> { }

public class DestinationCollector : MonoBehaviour
{
    [Header("Collection Rules")]
    [Tooltip("Only objects with this tag are considered samples.")]
    public string sampleTag = "Sample";

    [Tooltip("Require the rock to be dry before accepting.")]
    public bool requireDry = true;

    [Tooltip("Destroy the sample on collect (instead of leaving it in the bowl).")]
    public bool destroyOnCollect = true;

    [Header("Events")]
    public RockSampleEvent onCollected;

    [Header("Feedback")]
    public AudioSource collectSfx;

    void OnTriggerEnter(Collider other)
    {
        // Check tag on collider or its root
        if (!string.IsNullOrEmpty(sampleTag) && !other.CompareTag(sampleTag))
        {
            var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.transform.root.gameObject;
            if (!root.CompareTag(sampleTag))
                return;
        }

        var sample = other.GetComponentInParent<RockSample>();
        if (sample == null) return;
        if (requireDry && !sample.IsDry) return;

        onCollected?.Invoke(sample);
        if (collectSfx) collectSfx.Play();

        if (destroyOnCollect)
            Destroy(sample.gameObject);
    }
}
