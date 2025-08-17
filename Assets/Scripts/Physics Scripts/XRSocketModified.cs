// MODIFIED XRSocketInteractor
// Changes:
// 1. Snapping eligibility is determined by tag(s)
// 2. Audio plays once on successful snap
// 3. Prevent unapproved objects from being hovered or selected
// 4. Optionally disable convex on snap, and re-enable on release/grab again
// 5. Searches hierarchy for MeshCollider automatically
// 6. Supports multiple allowed tags

using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[AddComponentMenu("XR/Interactors/XR Socket Interactor With Audio (Multi-Tag)")]
public class XRSocketModified : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    [Header("Custom Socket Settings")]
    [Tooltip("Objects must match ANY of these tags to be considered for snapping. Leave empty to allow all.")]
    public string[] allowedTags = new[] { "Snappable" };

    [Tooltip("Audio source to play when an object successfully snaps.")]
    public AudioSource snapAudioSource;

    [Header("Snapped Object Adjustments")]
    [Tooltip("If true, the first MeshCollider found in the snapped object hierarchy will have convex turned OFF on snap, and back ON on release.")]
    public bool adjustColliderConvex = true;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable lastSnappedInteractable;
    private MeshCollider lastMeshCollider;

    // ----------------- Helpers -----------------
    bool IsAllowed(GameObject go)
    {
        if (go == null) return false;

        // If no tags configured (or only blanks), allow all
        if (allowedTags == null || allowedTags.Length == 0 ||
            allowedTags.All(t => string.IsNullOrWhiteSpace(t)))
            return true;

        // Match any tag (Unity tags are case-sensitive)
        for (int i = 0; i < allowedTags.Length; i++)
        {
            var tagStr = allowedTags[i];
            if (!string.IsNullOrEmpty(tagStr) && go.CompareTag(tagStr))
                return true;
        }
        return false;
    }

    // ----------------- XR Overrides -----------------
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;

        // Reject if tags don't match
        if (!IsAllowed(snappedObject))
        {
            Debug.Log($"[XRSocketModified] Rejected due to tag(s): {snappedObject.name}");
            interactionManager.SelectExit(this, args.interactableObject);
            return;
        }

        base.OnSelectEntered(args);

        if (args.interactableObject != lastSnappedInteractable)
        {
            lastSnappedInteractable = args.interactableObject;
            if (snapAudioSource != null) snapAudioSource.Play();
        }

        if (adjustColliderConvex && snappedObject != null)
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
        var target = interactable?.transform?.gameObject;
        if (!IsAllowed(target))
            return false;

        return base.CanHover(interactable);
    }

    public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        var target = interactable?.transform?.gameObject;
        if (!IsAllowed(target))
            return false;

        return base.CanSelect(interactable);
    }
}
