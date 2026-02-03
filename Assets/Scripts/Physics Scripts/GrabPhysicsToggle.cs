using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class GrabPhysicsToggle : MonoBehaviour
{
    Rigidbody _rb;
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        _grab.selectEntered.AddListener(_ => OnGrab());
        _grab.selectExited.AddListener(_ => OnRelease());
    }

    void OnDestroy()
    {
        if (_grab == null) return;
        _grab.selectEntered.RemoveListener(_ => OnGrab());
        _grab.selectExited.RemoveListener(_ => OnRelease());
    }

    void OnGrab()
    {
        // While held: let XR drive it
        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    void OnRelease()
    {
        // When released: let physics take over
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.WakeUp();
    }
}
