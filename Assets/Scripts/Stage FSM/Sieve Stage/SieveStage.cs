using UnityEngine;
using System.Collections;

public class SieveStage : StageBase, IShovelFlowHandler
{
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;

    public GameObject snapZone1;
    public GameObject snapZone2;
    public GameObject snapZone3;

    public GameObject bucketZoneObject;
    public GameObject sieveZoneObject;

    public GameObject bucketLayer;
    public GameObject sieveLayer;
    public GameObject shovelLayer;
    public GameObject stoneLayer;

    public HorizontalShakeTask horizontal;


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

        if (bucketZoneObject != null)
            bucketZoneObject.SetActive(false);

        if (sieveZoneObject != null)
            sieveZoneObject.SetActive(false);

        horizontal.enabled = true;

    }

    public void OnBucketSnapped(GameObject bucket)
    {
        // NA
    }


    public override void Enter()
    {
        IsComplete = false;
        audioPlayer.PlayAfterDelay(stageInstructions, 5f);

        snapZone1.SetActive(true);
        snapZone2.SetActive(true);
        snapZone3.SetActive(true);

        if (bucketZoneObject != null)
            bucketZoneObject.SetActive(true);

        if (sieveZoneObject != null)
            sieveZoneObject.SetActive(true);


    }

    public override void UpdateStage()
    {

        if (horizontal.isShakenComplete)
        {
            IsComplete = true;

            if(sieveLayer != null && stoneLayer != null)
            {
                sieveLayer.SetActive(false);
                stoneLayer.SetActive(true);
            }
          
        }

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
