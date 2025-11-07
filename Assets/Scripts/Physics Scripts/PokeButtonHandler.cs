using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRSimpleInteractable))]
public class PokeButtonHandler : MonoBehaviour
{
    [Header("Behavior")]
    [Tooltip("If true, press toggles ON/OFF. If false, it's momentary (true while poked).")]
    public bool toggleMode = false;

    [Header("State (read-only)")]
    [SerializeField] private bool isPressed = false;
    public bool IsPressed => isPressed;

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private XRSimpleInteractable simple;

    void Awake()
    {
        simple = GetComponent<XRSimpleInteractable>();
    }

    void OnEnable()
    {
        simple.selectEntered.AddListener(OnSelectEntered);
        simple.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        simple.selectEntered.RemoveListener(OnSelectEntered);
        simple.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (toggleMode)
        {
            isPressed = !isPressed;
            if (isPressed) onPressed?.Invoke();
            else onReleased?.Invoke();
        }
        else
        {
            if (!isPressed)
            {
                isPressed = true;
                onPressed?.Invoke();
            }
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (!toggleMode && isPressed)
        {
            isPressed = false;
            onReleased?.Invoke();
        }
    }

    // Optional: let other scripts force the state
    public void SetPressed(bool value)
    {
        if (isPressed == value) return;
        isPressed = value;
        if (isPressed) onPressed?.Invoke();
        else onReleased?.Invoke();
    }
}
