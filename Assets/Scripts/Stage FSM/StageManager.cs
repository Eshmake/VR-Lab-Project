using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private List<StageBase> stages = new List<StageBase>();
    private int currentStageIndex = 0;

    public event Action<StageBase> OnStageChanged;

    public StageBase CurrentStage => stages.Count > currentStageIndex ? stages[currentStageIndex] : null;

    void Start()
    {
        if (stages.Count == 0)
        {
            Debug.LogError("No stages assigned to StageManager.");
            return;
        }

        EnterCurrentStage();
    }

    public void StartStages()
    {
        currentStageIndex = 0;
        EnterCurrentStage();
    }

    void Update()
    {
        if (stages.Count == 0 || currentStageIndex >= stages.Count)
            return;

        var stage = stages[currentStageIndex];
        stage.UpdateStage();

        if (stage.IsComplete)
        {
            stage.Exit();
            currentStageIndex++;
            if (currentStageIndex < stages.Count)
            {
                EnterCurrentStage();
            }
            else
            {
                Debug.Log("All stages complete!");
            }
        }
    }

    private void EnterCurrentStage()
    {
        var stage = stages[currentStageIndex];
        stage.Enter();
        OnStageChanged?.Invoke(stage); // notify listeners
    }
}
