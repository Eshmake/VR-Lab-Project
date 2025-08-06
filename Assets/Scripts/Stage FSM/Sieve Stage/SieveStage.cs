using UnityEngine;
using System.Collections;

public class SieveStage : StageBase
{
    public AudioSource stageInstructions;
    public AudioDelayPlayer audioPlayer;


    public override void Enter()
    {
        IsComplete = false;
        audioPlayer.PlayAfterDelay(stageInstructions, 4f);


    }

    public override void UpdateStage()
    {


    }

    public override void Exit()
    {
        
    }

    public override string GetInstructionText()
    {
        return "";
    }


}
