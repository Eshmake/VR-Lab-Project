using UnityEngine;

public class DryStoneCollector : MonoBehaviour
{
    [Header("Collector Settings")]
    public string requiredTag = "DryStone";
    public GameObject gravelLayer;   // Optional visual that appears after enough stones

    private int collectedCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            collectedCount++;
            Destroy(other.gameObject);

            if (gravelLayer != null && !gravelLayer.activeSelf)
                gravelLayer.SetActive(true);

            // TODO: notify stage manager if needed
        }
    }
}
