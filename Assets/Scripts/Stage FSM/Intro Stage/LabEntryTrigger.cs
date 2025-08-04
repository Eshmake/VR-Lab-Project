using UnityEngine;

public class LabEntryTrigger : MonoBehaviour
{
    public IntroStage introStage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            introStage.NotifyEntry();
        }
    }
}
