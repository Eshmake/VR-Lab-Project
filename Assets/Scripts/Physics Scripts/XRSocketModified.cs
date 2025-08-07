// MODIFIED XRSocketInteractor
// Changes:
// 1. Snapping eligibility is determined by tag
// 2. Audio plays once on successful snap
// 3. Prevent untagged objects from being hovered or affected
// 4. Optionally disable convex on snap, and re-enable on release/grab again
// 5. Searches hierarchy for MeshCollider automatically

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[AddComponentMenu("XR/Interactors/XR Socket Interactor With Audio")]
public class XRSocketModified : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    [Header("Custom Socket Settings")]
    [Tooltip("Only objects with this tag will be considered for snapping.")]
    public string requiredTag = "Snappable";

    [Tooltip("Audio source to play when an object successfully snaps.")]
    public AudioSource snapAudioSource;

    [Header("Snapped Object Adjustments")]
    public bool adjustColliderConvex = true;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable lastSnappedInteractable;
    private MeshCollider lastMeshCollider;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Reject if tag does not match
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        if (!string.IsNullOrEmpty(requiredTag) && !snappedObject.CompareTag(requiredTag))
        {
            Debug.Log("Rejected due to tag: " + snappedObject.name);
            interactionManager.SelectExit(this, args.interactableObject);
            return;
        }

        base.OnSelectEntered(args);

        if (args.interactableObject != lastSnappedInteractable)
        {
            lastSnappedInteractable = args.interactableObject;

            if (snapAudioSource != null)
                snapAudioSource.Play();
        }

        if (adjustColliderConvex)
        {
            lastMeshCollider = snappedObject.GetComponentInChildren<MeshCollider>();

            if (lastMeshCollider != null)
                lastMeshCollider.convex = false;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        lastSnappedInteractable = null;

        if (adjustColliderConvex && lastMeshCollider != null)
        {
            lastMeshCollider.convex = true;
            lastMeshCollider = null;
        }
    }

    public override bool CanHover(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable)
    {
        GameObject target = interactable.transform.gameObject;
        if (!string.IsNullOrEmpty(requiredTag) && !target.CompareTag(requiredTag))
            return false;

        return base.CanHover(interactable);
    }

    public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        GameObject target = interactable.transform.gameObject;
        if (!string.IsNullOrEmpty(requiredTag) && !target.CompareTag(requiredTag))
            return false;

        return base.CanSelect(interactable);
    }
}
