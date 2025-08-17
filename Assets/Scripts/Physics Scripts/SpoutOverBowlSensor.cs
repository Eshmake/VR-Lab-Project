using UnityEngine;

public class SpoutOverBowlSensor : MonoBehaviour
{
    public FaucetController controller;
    public string bowlRimTag = "BowlRinseArea";

    void OnTriggerEnter(Collider other)
    {
        if (controller && other.CompareTag(bowlRimTag))
            controller.SetOverBowl(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (controller && other.CompareTag(bowlRimTag))
            controller.SetOverBowl(false);
    }
}
