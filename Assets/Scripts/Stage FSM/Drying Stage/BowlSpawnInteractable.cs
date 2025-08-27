using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class BowlSpawnInteractable : MonoBehaviour
{
    [Header("XR")]
    [Tooltip("Scene XRInteractionManager. If empty, will auto-find in Awake().")]
    public XRInteractionManager interactionManager;

    [Header("Spawning")]
    [Tooltip("Rock prefabs to spawn (must have Rigidbody, colliders, RockSample).")]
    public GameObject[] rockPrefabs;
    [Tooltip("Where the rock appears initially (child Transform inside the bowl).")]
    public Transform spawnPoint;
    [Tooltip("If true, waits until current rock is gone before allowing another.")]
    public bool onlyOneActiveAtATime = true;
    [Tooltip("Optional small delay to avoid race conditions (0 = next frame).")]
    public float deferSpawnSeconds = 0f;

    [Header("Events")]
    [Tooltip("Invoked when a rock is spawned (after it is handed to the interactor).")]
    public UnityEvent<GameObject> onRockSpawned = new UnityEvent<GameObject>();

    [Header("Rock Spawn Audio")]
    public AudioSource rockAudio;

    // runtime
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _zone;
    GameObject _currentActive;
    int _nextIndex;
    bool _spawning;

    void Awake()
    {
        _zone = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (!interactionManager)
            interactionManager = FindObjectOfType<XRInteractionManager>();
    }

    void OnEnable()
    {
        _zone.selectEntered.AddListener(OnZoneSelected);
        _zone.selectExited.AddListener(OnZoneDeselected);
    }

    void OnDisable()
    {
        _zone.selectEntered.RemoveListener(OnZoneSelected);
        _zone.selectExited.RemoveListener(OnZoneDeselected);
    }

    void OnZoneSelected(SelectEnterEventArgs args)
    {
        if (onlyOneActiveAtATime && _currentActive != null) return;
        if (_spawning) return;
        if (_nextIndex >= (rockPrefabs?.Length ?? 0)) return;
        if (!interactionManager || !spawnPoint) return;

        _spawning = true;
        StartCoroutine(SpawnAndHandOver(args.interactorObject));
    }

    void OnZoneDeselected(SelectExitEventArgs args)
    {
        // No-op. Selection is transferred in SpawnAndHandOver.
    }

    IEnumerator SpawnAndHandOver(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
    {
        // brief defer can help with timing in some XRI versions
        if (deferSpawnSeconds > 0f) yield return new WaitForSeconds(deferSpawnSeconds);
        else yield return null;

        _spawning = false;

        if (interactor == null || !interactionManager) yield break;
        if (_nextIndex >= (rockPrefabs?.Length ?? 0)) yield break;

        var prefab = rockPrefabs[_nextIndex];
        if (!prefab) yield break;

        // spawn
        var go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        _currentActive = go;
        _nextIndex++;

        // ensure grabbable + rigidbody (namespaces differ by XRI versions, so add via full name if missing)
        var grab = go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (!grab) grab = go.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (!go.TryGetComponent<Rigidbody>(out _)) go.AddComponent<Rigidbody>();

        // align to interactor’s attach pose for seamless pickup
        var attach = interactor.GetAttachTransform(grab);
        go.transform.SetPositionAndRotation(attach.position, attach.rotation);

        // transfer selection: GrabZone -> rock
        interactionManager.SelectExit(interactor, _zone);
        interactionManager.SelectEnter(interactor, grab);

        //let listeners (DryingStage) know a pickup happened
        onRockSpawned.Invoke(go);

        if(rockAudio != null)
        {
            rockAudio.Play();
        }

        // track lifecycle to allow re-spawn later
        var life = go.GetComponent<RockLifecycleNotifier>();
        if (!life) life = go.AddComponent<RockLifecycleNotifier>();
        life.onRockGone += () => { if (_currentActive == go) _currentActive = null; };
    }

    // Call if you need to restart sequence (e.g., when re-entering stage)
    public void ResetSpawner()
    {
        _currentActive = null;
        _nextIndex = 0;
        _spawning = false;
    }
}

