using UnityEngine;

public class WeighingStage : StageBase
{
    [Header("Audio (optional)")]
    public AudioSource stageInstructions;
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
    public float bowlWeight = 313.313f;
    public string weightFormat = "{0:0.000}";


    private bool IsSnapped = false;
    private string requiredTag = "Bowl 2";


    public override void Enter()
    {
    

        if (audioPlayer && stageInstructions)
            audioPlayer.PlayAfterDelay(stageInstructions, 5f);

        IsComplete = false;

        if(snapZone != null)
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

        if (audioPlayer && stageComplete)
            audioPlayer.PlayAfterDelay(stageComplete, 2f);

    }

    public override string GetInstructionText()
    {
        return "1. Grab red bowl with the dried aggregate, and snap it to the attach plate on top of the scale.\n\n2. Make note of the weight, as it is the value B in your final calculations.";
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
