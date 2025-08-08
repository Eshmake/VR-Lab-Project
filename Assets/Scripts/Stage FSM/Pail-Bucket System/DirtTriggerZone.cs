using UnityEngine;

public class DirtTriggerZone : MonoBehaviour
{
    public enum ZoneType { Pail, Bucket }

    [Header("Trigger Settings")]
    public ZoneType zoneType;

    [Tooltip("Trigger collider that detects entry. If null, defaults to this GameObject’s collider.")]
    public Collider zoneTrigger;

    [Tooltip("The specific collider on the shovel (e.g., shovel head) that should trigger this zone.")]
    public Collider shovelHeadCollider;

    [Tooltip("Any component that implements Pail-Bucket System")]
    public MonoBehaviour handlerBehaviour;
    public IShovelFlowHandler handler;

    public AudioSource test;
    public AudioDelayPlayer audioPlayer;



    public bool isActive = true;

    void Awake()
    {
        audioPlayer.PlayAfterDelay(test, 2f);
        handler = handlerBehaviour as IShovelFlowHandler;
        if (zoneTrigger == null)
            zoneTrigger = GetComponent<Collider>();

        if (zoneTrigger != null && !zoneTrigger.isTrigger)
            zoneTrigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        audioPlayer.PlayAfterDelay(test, 2f);

        if (!isActive || other != shovelHeadCollider || handler == null)
            return;

        audioPlayer.PlayAfterDelay(test, 2f);

        // Ensure the triggered collider is the assigned trigger
        if (zoneTrigger != null && other != shovelHeadCollider)
            return;

        audioPlayer.PlayAfterDelay(test, 2f);

        var shovel = shovelHeadCollider.GetComponentInParent<ShovelDirt>();
        if (shovel == null)
            return;

        audioPlayer.PlayAfterDelay(test, 2f);

        if (zoneType == ZoneType.Pail && !shovel.IsFull)
        {
            audioPlayer.PlayAfterDelay(test, 2f);
            shovel.Fill();
            handler.OnShovelFilled(shovel);
        }
        else if (zoneType == ZoneType.Bucket && shovel.IsFull)
        {
            audioPlayer.PlayAfterDelay(test, 2f);
            shovel.Empty();
            handler.OnShovelDumped(shovel);
        }
    }
}
