using UnityEngine;
using System.Collections;

public class SieveStage : StageBase
{
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;

    public GameObject snapZone1;
    public GameObject snapZone2;
    public GameObject snapZone3;

    public DirtTriggerZone bucketZone;
    public DirtTriggerZone sieveZone;

    public GameObject bucketLayer;
    public GameObject sieveLayer;
    public GameObject shovelLayer;


    public void OnShovelFilled(ShovelDirt shovel)
    {
        if (shovelLayer != null && bucketLayer != null)
        {
            bucketLayer.SetActive(false);
            shovelLayer.SetActive(true);
        }
        
    }

    public void OnShovelDumped(ShovelDirt shovel)
    {
        if (shovelLayer != null && sieveLayer != null)
        {
            shovelLayer.SetActive(false);
            sieveLayer.SetActive(true);
        }

        if (bucketZone != null)
            bucketZone.isActive = false;

        if (sieveZone != null)
            sieveZone.isActive = false;
    }


    public override void Enter()
    {
        IsComplete = false;
        audioPlayer.PlayAfterDelay(stageInstructions, 5f);

        snapZone1.SetActive(true);
        snapZone2.SetActive(true);
        snapZone3.SetActive(true);

        if (bucketZone != null)
            bucketZone.isActive = true;

        if (sieveZone != null)
            sieveZone.isActive = true;


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
        return "";
    }


}
