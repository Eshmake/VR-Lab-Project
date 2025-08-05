using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class GrabDropAudio : MonoBehaviour
{
    [Header("Sound Effects")]
    public AudioSource grabSound;
    public AudioSource dropSound;

    [Header("Collision Settings")]
    [Tooltip("If assigned, only collisions involving this collider will trigger the drop sound.")]
    public Collider dropTriggerCollider;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable;
    private Rigidbody rb;

    void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Hook into XR events
        interactable.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (grabSound != null)
            grabSound.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (dropSound == null)
            return;

        // If a specific collider is assigned, only play sound if it was involved
        if (dropTriggerCollider != null)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == dropTriggerCollider && collision.relativeVelocity.magnitude > 0.2f)
                {
                    dropSound.Play();
                    break;
                }
            }
        }
        else if (collision.relativeVelocity.magnitude > 0.2f)
        {
            dropSound.Play();
        }
    }
}

