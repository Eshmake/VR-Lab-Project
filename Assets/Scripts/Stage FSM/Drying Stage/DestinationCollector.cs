using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent<RockSample> onCollected;

    [Header("Feedback")]
    public AudioSource collectSfx;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(sampleTag))
            return;

        RockSample sample = other.GetComponentInParent<RockSample>();
        if (sample == null)
            return;

        if (requireDry && !sample.IsDry)
            return;

        // Trigger collection event
        onCollected?.Invoke(sample);

        if (collectSfx) collectSfx.Play();

        if (destroyOnCollect)
            Destroy(sample.gameObject);
    }
}

