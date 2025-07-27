using UnityEngine;

public class SmoothFacePlayer : MonoBehaviour
{
    public Transform playerHead; // Assign the camera or XR Rig center
    public float rotationSpeed = 2f;
    public float maxAngle = 45f; // Only rotate if within this angle

    void Update()
    {
        Vector3 directionToPlayer = playerHead.position - transform.position;
        directionToPlayer.y = 0; // Optional: only rotate around Y axis
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        if (angle < maxAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}