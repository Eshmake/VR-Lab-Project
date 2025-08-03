using UnityEngine;

public class GeneralSnapZone : MonoBehaviour
{
    [Header("Snap Settings")]
    public Transform snapTarget;
    public string requiredTag = "";
    public float snapSpeed = 8f;

    [Header("Visual Feedback")]
    public GameObject highlightObject;

    [Header("Audio")]
    public AudioSource snapAudio; // assign a 3D AudioSource with snap sound

    private Transform snappable;
    private Rigidbody snappableRb;
    private bool hasSnapped = false;

    private void OnTriggerEnter(Collider other)
    {
        if (snappable != null) return; // Already snapping something

        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null)
            return;

        snappable = other.transform;
        snappableRb = rb;
        hasSnapped = false;

        if (highlightObject != null)
            highlightObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (snappable != null && other.transform == snappable)
        {
            snappable = null;
            snappableRb = null;
            hasSnapped = false;

            if (highlightObject != null)
                highlightObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (snappable == null)
            return;

        float posDist = Vector3.Distance(snappable.position, snapTarget.position);
        float rotDist = Quaternion.Angle(snappable.rotation, snapTarget.rotation);

        if (posDist < 0.005f && rotDist < 5f && !hasSnapped)
        {
            // Final snap and clean up
            snappable.position = snapTarget.position;
            snappable.rotation = snapTarget.rotation;

            if (snappableRb != null)
            {
                snappableRb.linearVelocity = Vector3.zero;
                snappableRb.angularVelocity = Vector3.zero;
            }

            if (highlightObject != null)
                highlightObject.SetActive(false);

            if (snapAudio != null)
                snapAudio.Play();

            // Prevent further updates
            snappable = null;
            snappableRb = null;
            hasSnapped = true;

            return;
        }

        // Smooth snapping while still approaching
        snappable.position = Vector3.Lerp(snappable.position, snapTarget.position, Time.deltaTime * snapSpeed);
        snappable.rotation = Quaternion.Slerp(snappable.rotation, snapTarget.rotation, Time.deltaTime * snapSpeed);

        if (snappableRb != null)
        {
            snappableRb.linearVelocity = Vector3.zero;
            snappableRb.angularVelocity = Vector3.zero;
        }
    }
}
