using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabKinematicToggle : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (rb != null)
            rb.isKinematic = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (rb != null)
            rb.isKinematic = false;
    }

    void OnDestroy()
    {
        // Clean up listeners
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}
