using UnityEngine;

public class DirtTriggerZone : MonoBehaviour
{
    public enum ZoneType { Pail, Bucket }

    [Header("Trigger Settings")]
    public ZoneType zoneType;

    [Tooltip("Trigger collider on this object that should detect entry.")]
    public Collider zoneTrigger; // the trigger on the bucket or pail

    [Tooltip("The specific collider on the shovel (e.g., shovel head) that should trigger this zone.")]
    public Collider shovelHeadCollider;

    [Tooltip("The Stage Manager that tracks dirt fill/dump progress.")]
    public ShovelingStage stageManager;

    public bool isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || other != shovelHeadCollider || stageManager == null)
            return;

        // Make sure the trigger that was entered is our assigned one
        if (!zoneTrigger.bounds.Intersects(shovelHeadCollider.bounds))
            return;

        var shovel = shovelHeadCollider.GetComponentInParent<ShovelDirt>();
        if (shovel == null)
            return;

        if (zoneType == ZoneType.Pail && !shovel.IsFull)
        {
            shovel.Fill();
            stageManager.NotifyShovelFilled();
        }
        else if (zoneType == ZoneType.Bucket && shovel.IsFull)
        {
            shovel.Empty();
            stageManager.NotifyShovelDumped();
        }
    }
}
