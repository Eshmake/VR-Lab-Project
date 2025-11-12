using UnityEngine;
using System.Collections;


public class SubmergeStage : StageBase, IShovelFlowHandler
{
    [Header("Audio")]
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioSource finalNote;
    public AudioSource submergeSound;
    public AudioDelayPlayer audioPlayer;

    [Header("Snap Zone")]
    public GameObject snapZone;

    [Header("Snap Watcher")]
    public SocketWatcher scaleWatcher;

    [Header("Text Panel Controller Script")]
    public TextPanelController textChanger;

    [Header("Scale Text Bridge")]
    public ScaleReadoutBridge readoutBridge;

    [Header("Dirt Trigger Zones")]
    public GameObject hangZoneObject;
    public GameObject bowlZoneObject;

    [Header("Stone Layers")]
    public GameObject hangDryLayer;
    public GameObject hangWetLayer;
    public GameObject bowlLayer;
    public GameObject shovelLayer;


    private DirtTriggerZone hangZoneTrigger = null;
    private DirtTriggerZone bowlZoneTrigger = null;

    [Header("Weight Output")]
    public float hangWeight = 313.313f;
    public string weightFormat = "{0:0.000}";


    private bool IsSnapped = false;
    private bool IsFilled = false;


    private string requiredTag = "WireBucket";


    public override void Enter()
    {

        IsComplete = false;

        if (audioPlayer && stageInstructions)
            audioPlayer.PlayAfterDelay(stageInstructions, 5f);


        if (snapZone != null)
            snapZone.SetActive(true);


        if (hangZoneObject != null)
        {
            hangZoneTrigger = hangZoneObject.GetComponent<DirtTriggerZone>();
            hangZoneObject.SetActive(true);
            hangZoneTrigger.isActive = true;

        }


        if (bowlZoneObject != null)
        {
            bowlZoneTrigger = bowlZoneObject.GetComponent<DirtTriggerZone>();
            bowlZoneObject.SetActive(true);
            bowlZoneTrigger.isActive = true;
        }


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

        StartCoroutine(ExitRoutine());

    }

    private IEnumerator ExitRoutine()
    {
        if (audioPlayer && finalNote)
            audioPlayer.PlayAfterDelay(finalNote, 2f);

        yield return new WaitForSeconds(8f);

        if (scaleWatcher)
        {
            scaleWatcher.onSnapped.RemoveListener(OnBowlSnappedOnScale);
        }

        if (hangZoneObject != null)
            hangZoneObject.SetActive(false);

        if (bowlZoneObject != null)
            bowlZoneObject.SetActive(false);

        hangZoneObject = null;
        bowlZoneObject = null;

        if (audioPlayer && stageComplete)
            audioPlayer.PlayAfterDelay(stageComplete, 2f);


    }

    public override string GetInstructionText()
    {
        return "1. Snap both the red bowl and wire basket on top of the cabinets.\n\n2. Using the trowel, pour the sample from the bowl into the basket.\n\n3. Snap the basket below the scale in the barrel of water, to fully submerge it.\n\n4. Take note of the weight, as it is the value C in your final calculations.";
    }

    private void OnBowlSnappedOnScale(GameObject snapped)
    {
        if (snapped == null || !snapped.CompareTag(requiredTag) || !IsFilled)
        {
            return;
        }
        else
        {
            readoutBridge.ShowWeight(hangWeight, weightFormat);

            audioPlayer.PlayAfterDelay(submergeSound, 0.5f);

            hangDryLayer.SetActive(false);

            if(hangWetLayer != null)
                hangWetLayer.SetActive(true);

            IsSnapped = true;
        }


    }


    public void OnShovelFilled(ShovelDirt shovel)
    {
        
        if (shovelLayer != null && bowlLayer != null)
        {
            bowlLayer.SetActive(false);
            shovelLayer.SetActive(true);
        }
        

    }

    public void OnShovelDumped(ShovelDirt shovel)
    {
        
        if (shovelLayer != null && hangDryLayer != null)
        {
            shovelLayer.SetActive(false);
            hangDryLayer.SetActive(true);
        }

        hangZoneTrigger.isActive = false;
        bowlZoneTrigger.isActive = false;

        IsFilled = true;
       

    }

    public void OnBucketSnapped(GameObject bucket)
    {
        // NA
    }



}