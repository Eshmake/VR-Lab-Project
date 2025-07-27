using UnityEngine;
using UnityEngine.UI;

public class InstructionUI : MonoBehaviour
{
    public Text instructionText;
    public StageManager stageManager;

    void Start()
    {
        stageManager.OnStageChanged += UpdateInstruction;
        UpdateInstruction(stageManager.CurrentStage); // set initial
    }

    void OnDestroy()
    {
        stageManager.OnStageChanged -= UpdateInstruction;
    }

    void UpdateInstruction(StageBase newStage)
    {
        instructionText.text = newStage.GetInstructionText();
    }
}
