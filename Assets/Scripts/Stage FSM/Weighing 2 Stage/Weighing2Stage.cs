using UnityEngine;

public class Weighing2Stage : StageBase
{
    [Header("Audio (optional)")]
    public AudioSource stageInstructions1;
    public AudioSource stageInstructions2;

    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;


    [Header("Snap Zone")]
    public GameObject snapZone;

    [Header("Snap Watcher")]
    public SocketWatcher scaleWatcher;

    [Header("Text Panel Controller Script")]
    public TextPanelController textChanger;

    [Header("Scale Text Bridge")]
    public ScaleReadoutBridge readoutBridge;

    [Header("Weight Output")]
    public float bowlWeight = 3.984f;
    public string weightFormat = "{0:0.000}";


    private bool IsSnapped = false;
    private string requiredTag = "Bowl 2";


    public override void Enter()
    {


        if (audioPlayer && stageInstructions1)
            audioPlayer.PlayAfterDelay(stageInstructions1, 20f);
            // audio 1


        IsComplete = false;

        if (snapZone != null)
            snapZone.SetActive(true);


        if (scaleWatcher)
        {
            scaleWatcher.onSnapped.AddListener(OnBowlSnappedOnScale);
        }

    }

    public override void UpdateStage()
    {
        if (IsSnapped)
            IsComplete = true;

    }

    public override void Exit()
    {
        if (scaleWatcher)
        {
            scaleWatcher.onSnapped.RemoveListener(OnBowlSnappedOnScale);
        }

        audioPlayer.PlayAfterDelay(stageInstructions2, 1f);

        base.EndAudio();

        if (audioPlayer && stageComplete)
            audioPlayer.PlayAfterDelay(stageComplete, 5f);
            // audio 2

    }

    public override string GetInstructionText()
    {
        return "1. Remove the bowl from the oven, and snap it to the attach plate on top of the scale again to weigh the oven-dried sample.\n\n2. Take note of the weight, as it is the value A in your final calculations.";
    }

    private void OnBowlSnappedOnScale(GameObject snapped)
    {

        if (snapped == null || !snapped.CompareTag(requiredTag))
        {
            return;
        }
        else
        {
            readoutBridge.ShowWeight(bowlWeight, weightFormat);
            IsSnapped = true;
        }

    }



}
