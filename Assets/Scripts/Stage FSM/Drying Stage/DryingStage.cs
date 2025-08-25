using UnityEngine;

public class DryingStage : StageBase
{
    [Header("Audio (optional)")]
    public AudioSource stageInstructions;
    public AudioSource stageComplete;
    public AudioDelayPlayer audioPlayer;

    [Header("Flow")]
    public DestinationCollector destinationCollector;
    public int samplesRequired = 5;

    [Header("Visuals")]
    public GameObject initialGravelLayer;     // enables after first collection
    public GameObject[] gravelSteps;          // progressive visuals per sample

    public GameObject sourceGravelLayer;

    [Header("Grab Zone")]
    public GameObject grabZone;

    int _collected;

    // --- added internals (no public names changed) ---
    int _pickups;
    int _lastStepIndex = -1;
    BowlSpawnInteractable _spawner;

    public override void Enter()
    {
        IsComplete = false;
        _collected = 0;
        _pickups = 0;
        _lastStepIndex = -1;


        if (audioPlayer && stageInstructions)
            audioPlayer.PlayAfterDelay(stageInstructions, 5f);

        if (destinationCollector != null)
            destinationCollector.onCollected.AddListener(OnSampleCollected);

        // subscribe to source spawner (on the grabZone object)
        if (grabZone != null)
        {
            grabZone.SetActive(true);

            _spawner = grabZone.GetComponent<BowlSpawnInteractable>();
            if (_spawner == null)
                _spawner = grabZone.GetComponentInChildren<BowlSpawnInteractable>(true);

            if (_spawner != null)
            {
                // listen for pickups so we can hide source after 5th
                _spawner.onRockSpawned.AddListener(OnRockSpawnedFromSource);
            }
        }

        // reset visuals
        if (initialGravelLayer) initialGravelLayer.SetActive(false);
        if (gravelSteps != null) foreach (var g in gravelSteps) if (g) g.SetActive(false);
        if (sourceGravelLayer) sourceGravelLayer.SetActive(true);
    }

    public override void UpdateStage()
    {
        // Event-driven; nothing needed per frame
    }

    public override void Exit()
    {
        if (destinationCollector != null)
            destinationCollector.onCollected.RemoveListener(OnSampleCollected);

        if (_spawner != null)
            _spawner.onRockSpawned.RemoveListener(OnRockSpawnedFromSource);

        if (audioPlayer && stageComplete)
            audioPlayer.PlayAfterDelay(stageComplete, 2f);

        initialGravelLayer = null;
        sourceGravelLayer = null;
        grabZone = null;
        _spawner = null;
    }

    public override string GetInstructionText()
    {
        return $"Reach into the bowl to spawn a wet stone, dry it with the towel, then place it in the destination bowl ({_collected}/{samplesRequired}).";
    }

    // Called each time a rock is spawned into the player's hand from the source bowl
    void OnRockSpawnedFromSource(GameObject rockGO)
    {
        _pickups++;

        // After the 5th pickup, hide the source gravel
        if (sourceGravelLayer && _pickups >= 5)
            sourceGravelLayer.SetActive(false);
    }

    void OnSampleCollected(RockSample rock)
    {
        _collected = Mathf.Min(_collected + 1, samplesRequired);

        // Destination visuals: show ONLY the latest layer

        // Case 1: first collection uses initialGravelLayer
        if (_collected == 1)
        {
            if (initialGravelLayer) initialGravelLayer.SetActive(true);

            // ensure all step objects are off
            if (gravelSteps != null) foreach (var g in gravelSteps) if (g) g.SetActive(false);
            _lastStepIndex = -1;
        }
        else // _collected >= 2 → use gravelSteps progressively
        {
            // hide initial layer once we move past the first
            if (initialGravelLayer) initialGravelLayer.SetActive(false);

            if (gravelSteps != null && gravelSteps.Length > 0)
            {
                
                int idx = Mathf.Clamp(_collected - 2, 0, gravelSteps.Length - 1);

                // turn off the previously shown step
                if (_lastStepIndex >= 0 && _lastStepIndex < gravelSteps.Length)
                {
                    var prev = gravelSteps[_lastStepIndex];
                    if (prev) prev.SetActive(false);
                }

                // turn on the current step
                var curr = gravelSteps[idx];
                if (curr) curr.SetActive(true);

                _lastStepIndex = idx;
            }
        }

        if (_collected >= samplesRequired)
        {
            IsComplete = true;

            // Make sure source gravel is hidden even if player never picked exactly 5 before finishing
            if (sourceGravelLayer) sourceGravelLayer.SetActive(false);

            if (grabZone != null)
                grabZone.SetActive(false);
        }
    }
}

