using UnityEngine;

public class WashingStage : StageBase, IShovelFlowHandler
{
    [Header("Audio")]
    public AudioSource stageInstructions1;
    public AudioSource stageInstructions2;
    public AudioSource stageInstructions3;

    public AudioSource stageComplete;
    public AudioSource submerge;
    public AudioDelayPlayer audioPlayer;
   

    [Header("Zones that receive shovel pour events")]
    public GameObject bowlZoneObject;
    public GameObject sieveZoneObject;

    [Header("Snap Watchers")]
    public SocketWatcher sinkWatcher;
    public SocketWatcher[] counterWatchers;   // all counter snap plates

    [Header("Snap Zones")]
    public GameObject sinkSnapZone;
    public GameObject[] counterSnapZones;

    [Header("Layers")]
    public GameObject bowlLayerDry;
    public GameObject bowlLayerWet;
    public GameObject sieveLayer;
    public GameObject shovelLayer;

    [Header("Faucet")]
    public FaucetController faucet;        // controller that tracks IsOn & IsOverBowl

    // runtime
    DirtTriggerZone bowlZoneTrigger;
    DirtTriggerZone sieveZoneTrigger;
   
    private bool washComplete = false;
    private bool counterSnapAfterWash = false;
    private bool submergePlayed = false;

    // --- IShovelFlowHandler ---
    public void OnShovelFilled(ShovelDirt shovel)
    {
        if (sieveLayer && shovelLayer) { sieveLayer.SetActive(false); shovelLayer.SetActive(true); }
    }

    public void OnShovelDumped(ShovelDirt shovel)
    {
        if (bowlLayerDry && shovelLayer) { shovelLayer.SetActive(false); bowlLayerDry.SetActive(true); }

        bowlZoneTrigger.isActive = false;
        sieveZoneTrigger.isActive = false;

        audioPlayer.PlayAfterDelay(stageInstructions2, 1f);
        // audio 2
    }

    public void OnBucketSnapped(GameObject bucket) { /* N/A */ }

    public override void Enter()
    {
        IsComplete = false;
      

        if (faucet) faucet.ResetState();

        if (audioPlayer && stageInstructions1) audioPlayer.PlayAfterDelay(stageInstructions1, 5f);
        // audio 1

        if (counterSnapZones != null)
        {
            foreach (var z in counterSnapZones)
            {
                if (z != null) z.SetActive(true);
            }
        }

        if (sinkSnapZone != null) { sinkSnapZone.SetActive(true); }

        if (bowlZoneObject)
        {
            bowlZoneObject.SetActive(true);
            bowlZoneTrigger = bowlZoneObject.GetComponentInChildren<DirtTriggerZone>(true);
            if (bowlZoneTrigger) { bowlZoneTrigger.handlerBehaviour = this; bowlZoneTrigger.isActive = true; }
        }

        if (sieveZoneObject)
        {
            sieveZoneObject.SetActive(true);
            sieveZoneTrigger = sieveZoneObject.GetComponentInChildren<DirtTriggerZone>(true);
            if (sieveZoneTrigger) { sieveZoneTrigger.handlerBehaviour = this; sieveZoneTrigger.isActive = true; }
        }

        if (sinkWatcher)
        {
            sinkWatcher.onSnapped.AddListener(OnBowlSnappedInSink);
            sinkWatcher.onUnsnapped.AddListener(OnBowlUnsnappedFromSink);
        }
        if (counterWatchers != null)
        {
            foreach (var w in counterWatchers)
            {
                if (w == null) continue;
                w.onSnapped.AddListener(OnBowlSnappedOnCounter);
                w.onUnsnapped.AddListener(OnBowlUnsnappedFromCounter);
            }
        }

        if (faucet) faucet.ResetState();

        // initial layer state
        if (sieveLayer) sieveLayer.SetActive(true);
        if (shovelLayer) shovelLayer.SetActive(false);
        if (bowlLayerDry) bowlLayerDry.SetActive(false);
    }

    public override void UpdateStage()
    {
        if (faucet != null && faucet.IsWashComplete)
        {
            washComplete = true;

            if(!submergePlayed)
            {
                submerge.Play();
                submergePlayed = true;
            }
            
            if (bowlLayerWet != null)
            {
                bowlLayerDry.SetActive(false);
                bowlLayerWet.SetActive(true);
            }

            audioPlayer.PlayAfterDelay(stageInstructions3, 1f);
            // audio 3
        }
            
    }

    public override void Exit()
    {
        if (bowlZoneObject) bowlZoneObject.SetActive(false);
        if (sieveZoneObject) sieveZoneObject.SetActive(false);

        if (sinkWatcher)
        {
            sinkWatcher.onSnapped.RemoveListener(OnBowlSnappedInSink);
            sinkWatcher.onUnsnapped.RemoveListener(OnBowlUnsnappedFromSink);
        }
        if (counterWatchers != null)
        {
            foreach (var w in counterWatchers)
            {
                if (w == null) continue;
                w.onSnapped.RemoveListener(OnBowlSnappedOnCounter);
                w.onUnsnapped.RemoveListener(OnBowlUnsnappedFromCounter);
            }
        }

        if (audioPlayer && stageComplete) audioPlayer.PlayAfterDelay(stageComplete, 2f);
    }

    public override string GetInstructionText()
    {
        return "1. Snap sieve and blue bowl on top of the cabinets, and using the trowel, pour the sample from the sieve into the bowl.\n\n2. Snap bowl into the sink, and pour water onto sample until a bubble noise is heard.\n\n3. After bubble noise, return handle to its resting state, and snap the bowl back onto the counter.";
    }

    // --- Socket callbacks ---
    private void OnBowlSnappedInSink(GameObject snapped)
    {
        if (snapped != null && snapped.CompareTag("Bowl"))
            faucet?.SetBowlInSink(true);
    }

    private void OnBowlUnsnappedFromSink(GameObject snapped)
    {
        if (snapped != null && snapped.CompareTag("Bowl"))
            faucet?.SetBowlInSink(false);
    }

    private void OnBowlSnappedOnCounter(GameObject snapped)
    {
        // Ensure it's actually the bowl
        if (snapped == null || !snapped.CompareTag("Bowl"))
            return;

        if (washComplete)
        {
            counterSnapAfterWash = true;
            IsComplete = true;
        }
        else
        {
            // Snapped too early; must re-snap after wash
            counterSnapAfterWash = false;
        }
    }

    private void OnBowlUnsnappedFromCounter(GameObject snapped)
    {
        // Only reset our “post-wash” snap flag if it was the bowl
        if (snapped != null && snapped.CompareTag("Bowl"))
            counterSnapAfterWash = false;
    }
}

