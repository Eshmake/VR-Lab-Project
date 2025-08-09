using UnityEngine;
using System.Collections;

public class WashingStage : StageBase, IShovelFlowHandler
{
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;

    


    public void OnShovelFilled(ShovelDirt shovel)
    {
        

    }

    public void OnShovelDumped(ShovelDirt shovel)
    {
        

    }

    public void OnBucketSnapped(GameObject bucket)
    {
        // NA
    }


    public override void Enter()
    {
        IsComplete = false;
        audioPlayer.PlayAfterDelay(stageInstructions, 5f);


        


    }

    public override void UpdateStage()
    {

        

    }

    public override void Exit()
    {

        

        audioPlayer.PlayAfterDelay(stageComplete, 2f);

    }

    public override string GetInstructionText()
    {
        return "Pour stones into bowl, wash dirt in sink, and leave dirt sumbmerged in bowl on counter.";
    }


}
