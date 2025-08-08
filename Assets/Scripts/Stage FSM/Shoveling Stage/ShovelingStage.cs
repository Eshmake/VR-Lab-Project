using UnityEngine;
using System.Collections;

public class ShovelingStage : StageBase, IShovelFlowHandler
{
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;

    public GameObject snapZone;

    public GameObject[] bucketDirtLevels;

    private int dirtDeposits = 0;

    public GameObject bucketZoneObject;
    public GameObject pailZoneObject;

    public void OnShovelFilled(ShovelDirt shovel)
    {
        // NA
    }

    public void OnShovelDumped(ShovelDirt shovel)
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

    public void OnBucketSnapped(GameObject bucket)
    {
        // NA
    }

    public override void Enter()
    {
        IsComplete = false;
        audioPlayer.PlayAfterDelay(stageInstructions, 5f);

        snapZone.SetActive(true);

        dirtDeposits = 0;

        foreach (var dirt in bucketDirtLevels)
            dirt.SetActive(false);

        if (bucketZoneObject != null)
            bucketZoneObject.SetActive(true);

        if (pailZoneObject != null)
            pailZoneObject.SetActive(true);
    }

    public override void UpdateStage()
    {
        

    }

    public override void Exit()
    {
        if (bucketZoneObject != null)
            bucketZoneObject.SetActive(false);

        if (pailZoneObject != null)
            pailZoneObject.SetActive(false);

        audioPlayer.PlayAfterDelay(stageComplete, 2f);
    }

    public override string GetInstructionText()
    {
        return "Fill the bucket with dirt using the shovel.";
    }


}
