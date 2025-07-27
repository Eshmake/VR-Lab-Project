using UnityEngine;

public abstract class StageBase : MonoBehaviour
{
    public abstract void Enter();
    public abstract void UpdateStage();
    public abstract void Exit();
    public abstract string GetInstructionText();
    public bool IsComplete { get; protected set; }
}


