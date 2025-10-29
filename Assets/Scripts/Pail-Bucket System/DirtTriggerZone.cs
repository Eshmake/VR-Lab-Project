using UnityEngine;

public class DirtTriggerZone : MonoBehaviour
{
    public enum ZoneType { Pail, Bucket }

    [Header("Trigger Settings")]
    public ZoneType zoneType;

    [Tooltip("Trigger collider that detects entry. If null, defaults to this GameObject’s collider.")]
    public Collider zoneTrigger; // Can be on another GameObject

    [Tooltip("The specific collider on the shovel (e.g., shovel head) that should trigger this zone.")]
    public Collider shovelHeadCollider;

    [Tooltip("Any component that implements Pail-Bucket System")]
    public MonoBehaviour handlerBehaviour;
    public IShovelFlowHandler handler;

    public bool isActive = true;

    private void Awake()
    {
        handler = handlerBehaviour as IShovelFlowHandler;

        if (zoneTrigger == null)
            zoneTrigger = GetComponent<Collider>();

        if (zoneTrigger != null && !zoneTrigger.isTrigger)
            zoneTrigger.isTrigger = true;

        if (zoneTrigger != null)
        {
            // Add a forwarding helper if the collider isn't on this GameObject
            if (zoneTrigger.gameObject != gameObject)
            {
                var forwarder = zoneTrigger.gameObject.AddComponent<TriggerForwarder>();
                forwarder.target = this;
            }
        }
    }

    // This gets called directly or via TriggerForwarder
    public void HandleTriggerEnter(Collider other)
    {
        if (!isActive || handler == null)
            return;

        if (other != shovelHeadCollider)
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

    // Used when the collider is on the same GameObject as the script
    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerEnter(other);
    }
}

// Small helper to forward trigger events
public class TriggerForwarder : MonoBehaviour
{
    public DirtTriggerZone target;

    private void OnTriggerEnter(Collider other)
    {
        if (target != null)
            target.HandleTriggerEnter(other);
    }
}
