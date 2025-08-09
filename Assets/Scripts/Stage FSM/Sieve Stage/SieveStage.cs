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

    private DirtTriggerZone bucketZoneTrigger = null;
    private DirtTriggerZone sieveZoneTrigger = null;


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

        bucketZoneTrigger.isActive = false;
        sieveZoneTrigger.isActive = false;

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


        bucketZoneTrigger = bucketZoneObject.GetComponent<DirtTriggerZone>();
        sieveZoneTrigger = sieveZoneObject.GetComponent<DirtTriggerZone>();

    snapZone1.SetActive(true);
        snapZone2.SetActive(true);
        snapZone3.SetActive(true);

        if (bucketZoneObject != null)
        {
            bucketZoneObject.SetActive(true);
            bucketZoneTrigger.isActive = true;

        }
            

        if (sieveZoneObject != null)
        {
            sieveZoneObject.SetActive(true);
            sieveZoneTrigger.isActive = true;
        }


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

        if (bucketZoneObject != null)
            bucketZoneObject.SetActive(false);

        if (sieveZoneObject != null)
            sieveZoneObject.SetActive(false);

        bucketZoneObject = null;
        sieveZoneObject = null;

        audioPlayer.PlayAfterDelay(stageComplete, 2f);

    }

    public override string GetInstructionText()
    {
        return "Use the workbench to pour dirt from the bucket into the sieve, and then sieve the dirt";
    }


}
