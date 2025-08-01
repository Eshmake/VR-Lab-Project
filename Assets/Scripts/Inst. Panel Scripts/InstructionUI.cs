using UnityEngine;
using UnityEngine.UI;

public class InstructionUI : MonoBehaviour
{
    public StageManager stageManager;
    public Text instructionText; // Or TextMeshProUGUI if using TMP

    void OnEnable()
    {
        stageManager.OnStageChanged += HandleStageChanged;
    }

    void OnDisable()
    {
        stageManager.OnStageChanged -= HandleStageChanged;
    }

    private void HandleStageChanged(StageBase newStage)
    {
        if (instructionText != null)
        {
            instructionText.text = newStage.GetInstructionText();
        }
    }
}
