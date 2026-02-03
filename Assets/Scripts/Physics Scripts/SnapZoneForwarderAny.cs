using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapZoneForwarderAny : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
    public SieveStage sieveStage;

    private bool fired;

    private void Reset()
    {
        if (socket == null)
            socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    private void OnEnable()
    {
        fired = false;
        if (socket != null)
            socket.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (fired) return; // prevents double firing due to weirdness
        fired = true;

        var go = args.interactableObject.transform.gameObject;
        sieveStage?.OnAnyItemSnapped(go);
    }

    public void SetStage(SieveStage stage)
    {
        sieveStage = stage;
        fired = false;
    }

    // Optional: if you want to allow re-snap after removing, you can also listen to selectExited
}
