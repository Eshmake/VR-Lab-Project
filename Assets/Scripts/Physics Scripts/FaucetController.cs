using UnityEngine;

public class FaucetController : MonoBehaviour
{
    [Header("Detection")]
    public WaterRunDetector water;      // assign particle/audio detector
    public float requiredSeconds = 5f;  // time needed while valid

    [Header("Rules")]
    [Tooltip("Require the bowl to be snapped in the sink while timing.")]
    public bool requireBowlInSink = true;

    [Tooltip("If true, timer resets when conditions break (continuous). If false, time is cumulative.")]
    public bool resetWhenInvalid = false;

    [Header("State (read-only)")]
    public bool IsOverBowl { get; private set; }
    public bool IsBowlInSink { get; private set; }
    public float ElapsedWhileValid { get; private set; }
    public bool IsWashComplete { get; private set; }

    public void SetOverBowl(bool over) => IsOverBowl = over;
    public void SetBowlInSink(bool inSink) => IsBowlInSink = inSink;

    public void ResetState()
    {
        IsOverBowl = false;
        IsBowlInSink = false;
        ElapsedWhileValid = 0f;
        IsWashComplete = false;
    }

    void Update()
    {
        if (IsWashComplete || water == null) return;

        bool valid =
            (!requireBowlInSink || IsBowlInSink) &&
            IsOverBowl &&
            water.IsRunningNow();

        if (valid)
        {
            ElapsedWhileValid += Time.deltaTime;
            if (ElapsedWhileValid >= requiredSeconds)
                IsWashComplete = true;
        }
        else if (resetWhenInvalid)
        {
            ElapsedWhileValid = 0f;
        }
        // else: cumulative; pauses when invalid
    }
}
