using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapZoneForwarder : MonoBehaviour
{
    [Header("Socket on this snap plate")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    [Header("Stage to notify (set by StageManager or in inspector)")]
    public ShovelingStage shovelingStage;

    [Header("Optional: only allow buckets by tag")]
    public string bucketTag = "Bucket";

    private bool fired;

    private void Reset()
    {
        if (socket == null)
            socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    private void OnEnable()
    {
        fired = false;
        if (socket != null)
            socket.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (fired) return;

        var go = args.interactableObject.transform.gameObject;

        if (!string.IsNullOrEmpty(bucketTag) && !go.CompareTag(bucketTag))
            return;

        fired = true;

        if (shovelingStage != null)
            shovelingStage.OnBucketSnapped(go);
    }

    public void SetStage(ShovelingStage stage)
    {
        shovelingStage = stage;
        fired = false;
    }
}
