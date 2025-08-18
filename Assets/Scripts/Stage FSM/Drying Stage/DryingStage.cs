using UnityEngine;

public class DryingStage : StageBase
{
    [Header("Audio (optional)")]
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;

    [Header("Flow components")]
    [Tooltip("Collector on the destination bowl that consumes DRY samples.")]
    public DestinationCollector destinationCollector;

    [Tooltip("(Optional) Spawner that creates stones when the user grabs inside the source bowl.")]
    public MonoBehaviour spawnerBehaviour; // e.g., SpawnStoneOnEmptyGrab_Compat or SpawnStoneOnEmptyGrab

    [Header("Rules")]
    [Tooltip("How many dry samples must be inserted to finish this stage.")]
    public int samplesRequired = 5;

    [Header("Optional visuals")]
    [Tooltip("Shown after at least one sample is collected.")]
    public GameObject gravelLayer;
    [Tooltip("Progressive visual steps (index 0 is first sample, etc.).")]
    public GameObject[] gravelSteps;

    // runtime
    private int _collected;

    public override void Enter()
    {
        IsComplete = false;
        _collected = 0;

        // (Optional) play instructions with a small delay
        if (audioPlayer && stageInstructions)
            audioPlayer.PlayAfterDelay(stageInstructions, 2f);

        // Wire collector event
        if (destinationCollector != null)
            destinationCollector.onCollected.AddListener(OnSampleCollected);

        // Init visuals
        if (gravelLayer) gravelLayer.SetActive(false);
        if (gravelSteps != null)
            foreach (var g in gravelSteps) if (g) g.SetActive(false);
    }

    public override void UpdateStage()
    {
        // Event-driven; no per-frame logic needed here.
    }

    public override void Exit()
    {
        if (destinationCollector != null)
            destinationCollector.onCollected.RemoveListener(OnSampleCollected);

        if (audioPlayer && stageComplete)
            audioPlayer.PlayAfterDelay(stageComplete, 2f);
    }

    public override string GetInstructionText()
    {
        return $"Reach into the bowl to grab a wet stone, dry it with the towel, then insert it into the destination bowl. ({_collected}/{samplesRequired})";
    }

    private void OnSampleCollected(RockSample rock)
    {
        // DestinationCollector guarantees the rock is dry (requireDry = true)
        _collected = Mathf.Min(_collected + 1, samplesRequired);

        // Optional visuals
        if (gravelLayer && _collected > 0)
            gravelLayer.SetActive(true);

        if (gravelSteps != null && _collected - 1 < gravelSteps.Length)
        {
            var step = gravelSteps[_collected - 1];
            if (step) step.SetActive(true);
        }

        if (_collected >= samplesRequired)
            IsComplete = true;
    }
}
