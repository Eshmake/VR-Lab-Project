using UnityEngine;

public class WeighingStage : StageBase
{
    [Header("Audio (optional)")]
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;
   

    public override void Enter()
    {
        IsComplete = false;
        
    }

    public override void UpdateStage()
    {
        // Event-driven; nothing needed per frame
    }

    public override void Exit()
    {
        

        if (audioPlayer && stageComplete)
            audioPlayer.PlayAfterDelay(stageComplete, 2f);

    }

    public override string GetInstructionText()
    {
        return "";
    }

    
   
}
