using UnityEngine;

public class DryingStage : StageBase
{
    [Header("Audio (optional)")]
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;

    [Header("Flow")]
    public DestinationCollector destinationCollector;
    public int samplesRequired = 5;

    [Header("Visuals")]
    public GameObject initialGravelLayer;     // enables after first collection
    public GameObject[] gravelSteps;   // progressive visuals per sample

    public GameObject sourceGravelLayer;

    int _collected;

    public override void Enter()
    {
        IsComplete = false;
        _collected = 0;

        if (audioPlayer && stageInstructions)
            audioPlayer.PlayAfterDelay(stageInstructions, 5f);

        if (destinationCollector != null)
            destinationCollector.onCollected.AddListener(OnSampleCollected);

        if (initialGravelLayer) initialGravelLayer.SetActive(false);
        if (gravelSteps != null) foreach (var g in gravelSteps) if (g) g.SetActive(false);

        if(sourceGravelLayer) sourceGravelLayer.SetActive(true);
    }

    public override void UpdateStage()
    {
        // Event-driven; nothing needed per frame
    }

    public override void Exit()
    {
        if (destinationCollector != null)
            destinationCollector.onCollected.RemoveListener(OnSampleCollected);

        if (audioPlayer && stageComplete)
            audioPlayer.PlayAfterDelay(stageComplete, 2f);

        initialGravelLayer = null;
        sourceGravelLayer = null;
    }

    public override string GetInstructionText()
    {
        return $"Reach into the bowl to spawn a wet stone, dry it with the towel, then place it in the destination bowl ({_collected}/{samplesRequired}).";
    }

    void OnSampleCollected(RockSample rock)
    {
        _collected = Mathf.Min(_collected + 1, samplesRequired);

        if (initialGravelLayer && _collected == 1)
            initialGravelLayer.SetActive(true);

        if (_collected > 1)
            initialGravelLayer.SetActive(false);

        if (gravelSteps != null && _collected - 1 < gravelSteps.Length)
        {
            var step = gravelSteps[_collected - 1];
            if (step) step.SetActive(true);

            if(_collected > 2)
                gravelSteps[_collected - 2].SetActive(false);
        }

        if (_collected >= samplesRequired)
            IsComplete = true;
            sourceGravelLayer.SetActive(false);
    }
}
