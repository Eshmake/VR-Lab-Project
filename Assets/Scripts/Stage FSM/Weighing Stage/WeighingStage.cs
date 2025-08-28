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
        return "test stage 5";
    }

    private void OnBowlSnappedOnScale(GameObject snapped)
    {

        if (snapped == null || !snapped.CompareTag(requiredTag))
            return;
        else
        {
            textChanger.setSSDText();
            IsSnapped = true;
        }
           
    }



}
