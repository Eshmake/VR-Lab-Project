using UnityEngine;
using UnityEngine.Events;

public class ScaleReadoutBridge : MonoBehaviour
{
    [Header("Wiring")]
    public SocketWatcher scaleWatcher;          // your existing watcher on the scale socket
    public TextPanelController textChanger;     // your existing text controller

    [Header("Optional filtering")]
    public string requiredTag = "";             // e.g., "Bowl2"; blank = accept any

    [Header("Idle/zero")]
    public string zeroText = "0.000";

    [Header("Events for stage logic")]
    public UnityEvent onBowlSnapped;            // stage can listen here
    public UnityEvent onBowlUnsnapped;          // optional

    private bool _isBowlSnapped;
    public bool IsBowlSnapped => _isBowlSnapped;

    void OnEnable()
    {
        if (scaleWatcher != null)
        {
            scaleWatcher.onSnapped.AddListener(OnSnapped);
            scaleWatcher.onUnsnapped.AddListener(OnUnsnapped);
        }
        else
        {
            Debug.LogWarning("[ScaleReadoutBridge] scaleWatcher not assigned.");
        }

        // Ensure we start at zero
        ResetToZero();
    }

    void OnDisable()
    {
        if (scaleWatcher != null)
        {
            scaleWatcher.onSnapped.RemoveListener(OnSnapped);
            scaleWatcher.onUnsnapped.RemoveListener(OnUnsnapped);
        }
    }

    void OnSnapped(GameObject go)
    {
        if (!string.IsNullOrEmpty(requiredTag) && (go == null || !go.CompareTag(requiredTag)))
            return;

        _isBowlSnapped = true;
        onBowlSnapped?.Invoke();
        // NOTE: we do NOT set a weight here; the stage will call ShowWeight(...)
    }

    void OnUnsnapped(GameObject go)
    {
        _isBowlSnapped = false;
        ResetToZero();
        onBowlUnsnapped?.Invoke();
    }

    // --- API the stage can call ---

    public void ShowWeight(float value, string format = "{0:0.000}")
    {
        if (textChanger != null)
            textChanger.setWeightText(value, format);
    }

    public void ResetToZero()
    {
        if (textChanger != null)
            textChanger.setZeroText(zeroText);
    }
}
