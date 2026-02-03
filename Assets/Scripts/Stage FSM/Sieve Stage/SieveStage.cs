using UnityEngine;
using System.Collections;

public class SieveStage : StageBase, IShovelFlowHandler
{
    public AudioSource stageInstructions1;
    public AudioSource stageInstructions2;
    public AudioSource stageInstructions3;



    public AudioSource stageComplete;
    public AudioSource sieveShakeLoop;

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


    private bool bucketSnapped;
    private bool sieveSnapped;

    private GameObject snappedBucket;
    private GameObject snappedSieve;

    private bool bothSnappedAudioPlayed;


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

        audioPlayer.PlayAfterDelay(stageInstructions3, 1f);
        // audio 3

    }

    public void OnBucketSnapped(GameObject bucket)
    {
        // NA
    }


    public void OnAnyItemSnapped(GameObject go)
    {
        // Identify type by TAG (recommended)
        if (!bucketSnapped && go.CompareTag("Snappable"))
        {
            bucketSnapped = true;
            snappedBucket = go;
        }
        else if (!sieveSnapped && go.CompareTag("Sieve"))
        {
            sieveSnapped = true;
            snappedSieve = go;
        }
        else
        {
            // Something else snapped, or it was a duplicate bucket/sieve snap
            return;
        }

        // When BOTH are snapped, do your “proceed” behavior once
        if (bucketSnapped && sieveSnapped && !bothSnappedAudioPlayed)
        {
            bothSnappedAudioPlayed = true;

            audioPlayer.PlayAfterDelay(stageInstructions2, 1f);
            // audio 2

        }
    }



    public override void Enter()
    {
        audioPlayer.SetScope(audioScope);

        IsComplete = false;
        audioPlayer.PlayAfterDelay(stageInstructions1, 5f);
        // audio 1


        bucketSnapped = false;
        sieveSnapped = false;
        snappedBucket = null;
        snappedSieve = null;
        bothSnappedAudioPlayed = false;


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


        var f1 = snapZone1.GetComponent<SnapZoneForwarderAny>();
        if (f1 != null) f1.SetStage(this);

        var f2 = snapZone2.GetComponent<SnapZoneForwarderAny>();
        if (f2 != null) f2.SetStage(this);

        var f3 = snapZone3.GetComponent<SnapZoneForwarderAny>();
        if (f3 != null) f3.SetStage(this);

    }

    public override void UpdateStage()
    {

        if (horizontal.IsCurrentlyShaking)
        {
            if(sieveShakeLoop != null && !sieveShakeLoop.isPlaying)
            {
                sieveShakeLoop.loop = true;
                sieveShakeLoop.Play();
            }
        }
        else
        {
            if(sieveShakeLoop != null && sieveShakeLoop.isPlaying)
            {
                sieveShakeLoop.Stop();
            }
        }

        if (horizontal.isShakenComplete)
        {
            IsComplete = true;

            if(sieveShakeLoop != null &&  sieveShakeLoop.isPlaying)
            {
                sieveShakeLoop.Stop();
            }

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

        base.EndAudio();

        audioPlayer.PlayAfterDelay(stageComplete, 2f);

    }

    public override string GetInstructionText()
    {
        return "1. Snap the bucket and sieve to the attach plates on the cabinets.\n\n2. Using the trowel pour one sample from the bucket into the sieve.\n\n3. Shake the sieve until smaller material in the aggregate is sufficiently filtered.";
    }


}
