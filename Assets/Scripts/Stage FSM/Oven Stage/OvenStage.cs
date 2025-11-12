using UnityEngine;
using System.Collections;


public class OvenStage : StageBase, IShovelFlowHandler
{
    [Header("Audio")]
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioSource finalNote;
    public AudioSource ovenSound;
    public AudioSource ovenDone;
    public AudioDelayPlayer audioPlayer;

    [Header("Snap Zone")]
    public GameObject snapZone;

    [Header("Snap Watcher")]
    public SocketWatcher snapWatcher;

    [Header("Text Panel Controller Script")]
    public TextPanelController textChanger;

    [Header("Scale Text Bridge")]
    public ScaleReadoutBridge readoutBridge;

    [Header("Dirt Trigger Zones")]
    public GameObject hangZoneObject;
    public GameObject bowlZoneObject;

    [Header("Stone Layers")]
    public GameObject hangWetLayer;
    public GameObject bowlWetLayer;
    public GameObject bowlDryLayer;
    public GameObject shovelLayer;


    public PokeButtonHandler pokeButton;


    private DirtTriggerZone hangZoneTrigger = null;
    private DirtTriggerZone bowlZoneTrigger = null;

    [Header("Weight Output")]
    public float hangWeight = 313.313f;
    public string weightFormat = "{0:0.000}";


    private bool IsSnapped = false;
    private bool IsFilled = false;
    private bool IsPoked = false;


    private string requiredTag = "Bowl 2";


    public override void Enter()
    {

        IsComplete = false;

        if (audioPlayer && stageInstructions)
            audioPlayer.PlayAfterDelay(stageInstructions, 14f);


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


        if (snapWatcher)
        {
            snapWatcher.onSnapped.AddListener(OnBowlSnappedOnScale);
        }

    }


    

    void Update()
    {
        if(pokeButton != null && pokeButton.IsPressed && IsSnapped)
        {
            StartCoroutine(SnapRoutine());
            IsPoked = true;
        }
    }

    public override void UpdateStage()
    {
        if (IsPoked)
            IsComplete = true;

    }

    public override void Exit()
    {

        StartCoroutine(ExitRoutine());

    }

    private IEnumerator ExitRoutine()
    {
        if (audioPlayer && finalNote)
            audioPlayer.PlayAfterDelay(finalNote, 3f);

        yield return new WaitForSeconds(17f);

        if (snapWatcher)
        {
            snapWatcher.onSnapped.RemoveListener(OnBowlSnappedOnScale);
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
        return "1. Snap both the basket and red bowl on top of the cabinets.\n\n2. Using the trowel, pour the sample from the bucket back into the red bowl.\n\n3. Snap the red bowl to the attach plate inside the oven, close the oven doors, and press the red button.";
    }

    private void OnBowlSnappedOnScale(GameObject snapped)
    {
        if (snapped == null || !snapped.CompareTag(requiredTag) || !IsFilled)
        {
            return;
        }
        else
        {
            IsSnapped = true;
        }

    }


    IEnumerator SnapRoutine()
    {
        ovenSound.Play();
        yield return new WaitForSeconds(14f);
        ovenSound.Stop();

        bowlWetLayer.SetActive(false);
        bowlDryLayer.SetActive(true);

        ovenDone.Play();
    }


    public void OnShovelFilled(ShovelDirt shovel)
    {

        if (shovelLayer != null && hangWetLayer != null)
        {
            hangWetLayer.SetActive(false);
            shovelLayer.SetActive(true);
        }


    }

    public void OnShovelDumped(ShovelDirt shovel)
    {

        if (shovelLayer != null && bowlWetLayer != null)
        {
            shovelLayer.SetActive(false);
            bowlWetLayer.SetActive(true);
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