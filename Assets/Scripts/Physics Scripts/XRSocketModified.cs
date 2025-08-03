// MODIFIED XRSocketInteractor
// Changes:
// 1. Snapping eligibility is determined by tag
// 2. Audio plays once on successful snap
// 3. Prevent untagged objects from being hovered or affected

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[AddComponentMenu("XR/Interactors/XR Socket Interactor With Audio")]
public class XRSocketInteractorWithAudio : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    [Header("Custom Socket Settings")]
    [Tooltip("Only objects with this tag will be considered for snapping.")]
    public string requiredTag = "Snappable";

    [Tooltip("Audio source to play when an object successfully snaps.")]
    public AudioSource snapAudioSource;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable lastSnappedInteractable;

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

        // Prevent duplicate sounds on re-snap
        if (args.interactableObject != lastSnappedInteractable)
        {
            lastSnappedInteractable = args.interactableObject;
            if (snapAudioSource != null)
                snapAudioSource.Play();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        lastSnappedInteractable = null; // Reset so snap sound can play again next time
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
