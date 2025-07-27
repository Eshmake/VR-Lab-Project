using UnityEngine;

public class ShovelingStage : StageBase
{
    public override void Enter()
    {
        // Enable shovel, spawn bucket, set instructions
    }

    public override void UpdateStage()
    {
        // Check if enough dirt is in the bucket
    }

    public override void Exit()
    {
        // Clean up objects, reset
    }

    public override string GetInstructionText()
    {
        return "Use the shovel to fill the bucket with dirt.";
    }
}