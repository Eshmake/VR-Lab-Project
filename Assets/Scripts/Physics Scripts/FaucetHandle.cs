using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class FaucetHandle : MonoBehaviour
{
    [Header("Rotation → ON Logic")]
    [Tooltip("Angle difference (deg) from the offRotation that counts as ON.")]
    public float onAngleThreshold = 20f;

    [Tooltip("Captured at Awake() unless you set it explicitly in the inspector.")]
    public Quaternion offRotation;

    [SerializeField] private bool isOn;
    public bool IsOn => isOn;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (offRotation == Quaternion.identity)
            offRotation = transform.localRotation;
    }

    void Update()
    {
        float angle = Quaternion.Angle(offRotation, transform.localRotation);
        isOn = angle > onAngleThreshold;
    }
}
