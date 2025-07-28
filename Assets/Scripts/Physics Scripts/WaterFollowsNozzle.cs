using UnityEngine;

public class WaterFollowsNozzle : MonoBehaviour
{
    [Tooltip("The rotating sink nozzle")]
    public Transform nozzle;

    [Tooltip("Optional local offset from the nozzle (e.g., for stream origin)")]
    public Vector3 positionOffset = Vector3.zero;

    void LateUpdate()
    {
        if (nozzle == null) return;

        // Match rotation
        transform.rotation = nozzle.rotation;

        // Follow position (with optional offset in nozzle's local space)
        transform.position = nozzle.TransformPoint(positionOffset);
    }
}
