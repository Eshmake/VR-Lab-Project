using UnityEngine;
using System.Collections;

public class ShovelingStage : StageBase
{
    public AudioSource stageInstructions;
    public AudioDelayPlayer audioPlayer;

    public GameObject snapZone;

    public GameObject[] bucketDirtLevels;

    private int dirtDeposits = 0;

    public DirtTriggerZone bucketZone;
    public DirtTriggerZone pailZone;

    public void NotifyShovelFilled()
    {
        // Optional: play pail scoop SFX or animation
    }

    public void NotifyShovelDumped()
    {

        if (dirtDeposits < bucketDirtLevels.Length)
        {
            bucketDirtLevels[dirtDeposits].SetActive(true);
            dirtDeposits++;

            if (dirtDeposits == bucketDirtLevels.Length)
            {
                IsComplete = true;

            }
        }
    }

    public override void Enter()
    {
        IsComplete = false;
        audioPlayer.PlayAfterDelay(stageInstructions, 4f);

        snapZone.SetActive(true);

        dirtDeposits = 0;

        foreach (var dirt in bucketDirtLevels)
            dirt.SetActive(false);

    }

    public override void UpdateStage()
    {
        

    }

    public override void Exit()
    {
        if(bucketZone != null)
            bucketZone.isActive = false;
        
        if(pailZone != null)
            pailZone.isActive = false; 
    }

    public override string GetInstructionText()
    {
        return "Fill the bucket with dirt using the shovel.";
    }


}
