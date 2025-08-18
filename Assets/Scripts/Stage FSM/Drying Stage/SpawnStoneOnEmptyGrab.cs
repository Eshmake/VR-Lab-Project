using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnStoneOnEmptyGrab_Compat : MonoBehaviour
{
    [Header("XR")]
    public XRInteractionManager interactionManager;     // drag your scene XRInteractionManager
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] hands;                    // Left/Right XRDirectInteractor (base type OK)
    public ActionBasedController[] controllers;         // Left/Right controllers (for select input)

    [Header("Bowl Area (pick one)")]
    public Collider bowlVolume;                         // preferred: collider matching the bowl interior
    public Transform bowlCenter;                        // fallback: center + radius
    public float bowlRadius = 0.25f;

    [Header("Spawning")]
    public GameObject[] rockPrefabs;
    public Transform spawnPoint;
    public bool onlyOneActiveAtATime = true;
    public float endOfFrameDeferral = 0f;               // 0 => next frame; >0 => wait seconds

    int nextIndex = 0;
    bool spawnPending = false;
    GameObject currentActive;

    // input edge detect
    bool[] prevPressed;

    void Awake()
    {
        if (!interactionManager)
            interactionManager = FindObjectOfType<XRInteractionManager>();

        if (hands == null) hands = new UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[0];
        if (controllers == null) controllers = new ActionBasedController[0];

        prevPressed = new bool[controllers.Length];
    }

    void Update()
    {
        for (int i = 0; i < controllers.Length; i++)
        {
            var ctrl = controllers[i];
            var hand = (i < hands.Length) ? hands[i] : null;
            if (!ctrl || !hand || !interactionManager) continue;

            // InputActionProperty is a struct → no null-conditional here
            var action = ctrl.selectAction.action;
            bool isPressed = action != null && action.IsPressed();

            // rising edge
            if (!prevPressed[i] && isPressed)
            {
                if (!hand.hasSelection && HandInsideBowl(hand))
                    TrySpawnIntoHand(hand);
            }

            prevPressed[i] = isPressed;
        }
    }

    bool HandInsideBowl(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor hand)
    {
        Vector3 p = hand.transform.position;

        if (bowlVolume != null)
        {
            Vector3 closest = bowlVolume.ClosestPoint(p);
            // inside when ClosestPoint == p
            return (closest - p).sqrMagnitude < 1e-6f;
        }

        if (bowlCenter != null)
            return Vector3.Distance(p, bowlCenter.position) <= bowlRadius;

        return false;
    }

    void TrySpawnIntoHand(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor hand)
    {
        if (spawnPending) return;
        if (onlyOneActiveAtATime && currentActive != null) return;
        if (nextIndex >= (rockPrefabs?.Length ?? 0)) return;

        spawnPending = true;
        StartCoroutine(SpawnAndSelect_Coro(hand));
    }

    IEnumerator SpawnAndSelect_Coro(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor hand)
    {
        // let XRI/input settle
        if (endOfFrameDeferral <= 0f) yield return null;
        else yield return new WaitForSeconds(endOfFrameDeferral);

        spawnPending = false;

        if (!hand || !interactionManager) yield break;
        if (nextIndex >= (rockPrefabs?.Length ?? 0)) yield break;
        if (!spawnPoint) yield break;

        var prefab = rockPrefabs[nextIndex];
        if (!prefab) yield break;

        var go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        currentActive = go;
        nextIndex++;

        // ensure wet state if using your RockSample
        var sample = go.GetComponent<RockSample>();
        if (sample != null)
        {
            // if your RockSample has ResetToWet(), call it; otherwise omit
            // sample.ResetToWet();
        }

        // ensure grabbable & rigidbody (namespaces differ by XRI version, so keep this generic)
        var grab = go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable>() as Component;
        if (grab == null)
        {
            // Try add XRGrabInteractable from either namespace
            var added = go.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            grab = added;
        }
        if (go.GetComponent<Rigidbody>() == null)
            go.AddComponent<Rigidbody>();

        // place at the hand’s attach transform for seamless pickup
        var ixrInteractable = grab as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable;
        var attach = hand.GetAttachTransform(ixrInteractable);
        go.transform.SetPositionAndRotation(attach.position, attach.rotation);

        // programmatic select using the interface-based overload (fixes CS0619)
        var selectInteractor = hand as UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor;
        var selectInteractable = grab as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable;
        if (selectInteractor != null && selectInteractable != null)
            interactionManager.SelectEnter(selectInteractor, selectInteractable);

        // track when currentActive is gone
        var life = go.GetComponent<RockLifecycleNotifier>();
        if (life == null) life = go.AddComponent<RockLifecycleNotifier>();
        life.onRockGone += () => { if (currentActive == go) currentActive = null; };
    }
}
