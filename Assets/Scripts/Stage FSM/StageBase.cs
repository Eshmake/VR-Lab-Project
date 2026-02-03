using UnityEngine;

public abstract class StageBase : MonoBehaviour
{
    [SerializeField] protected AudioDelayPlayer audioPlayer;
    [SerializeField] protected StageAudioScope audioScope;

    public abstract void Enter();
    public abstract void UpdateStage();
    public abstract void Exit();
    public abstract string GetInstructionText();
    public bool IsComplete { get; protected set; }

    public virtual void EndAudio()
    {
        audioPlayer?.CancelAllDelayed(); // stops future audio that hasn't started yet
        audioScope?.StopAll();           // stops any stage audio already playing
    }
    
}


