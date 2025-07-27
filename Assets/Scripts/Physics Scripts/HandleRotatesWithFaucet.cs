using UnityEngine;

public class HandleRotatesWithFaucet : MonoBehaviour
{
    [Tooltip("The faucet base (rotating) part to follow")]
    public Transform faucetTransform;

    [Tooltip("Optional: the object used to rotate the handle (e.g., via HingeJoint)")]
    public Transform handlePivot; // If using a pivot joint under this object

    private Vector3 initialLocalPosition;

    void Start()
    {
        initialLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (faucetTransform == null) return;

        // Keep the position locked to its local position relative to faucet
        transform.position = faucetTransform.TransformPoint(initialLocalPosition);

        // Combine faucet rotation with handle's own local rotation
        Quaternion faucetRotation = faucetTransform.rotation;
        Quaternion handleLocalRotation = handlePivot != null ? handlePivot.localRotation : Quaternion.identity;

        transform.rotation = faucetRotation * handleLocalRotation;
    }
}
