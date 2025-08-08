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

    [Tooltip("Any component that implements Pail-Bucket System")]
    public MonoBehaviour handlerBehaviour;
    public IShovelFlowHandler handler;

    public bool isActive = true;


    void Awake()
    {
        handler = handlerBehaviour as IShovelFlowHandler;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || other != shovelHeadCollider || handler == null)
            return;

        // Make sure the trigger that was entered is our assigned one
        if (zoneTrigger != null && !zoneTrigger.bounds.Intersects(shovelHeadCollider.bounds))
            return;

        var shovel = shovelHeadCollider.GetComponentInParent<ShovelDirt>();
        if (shovel == null)
            return;

        if (zoneType == ZoneType.Pail && !shovel.IsFull)
        {
            shovel.Fill();
            handler.OnShovelFilled(shovel);
        }
        else if (zoneType == ZoneType.Bucket && shovel.IsFull)
        {
            shovel.Empty();
            handler.OnShovelDumped(shovel);
        }
    }
}
