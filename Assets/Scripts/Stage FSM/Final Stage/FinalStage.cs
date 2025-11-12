using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinalStage : StageBase
{
    public AudioSource finalAudio;
    public AudioDelayPlayer audioPlayer;

    public GameObject resultsPanel;


    private bool IsClicked = false;
 

    public override void Enter()
    {
        resultsPanel.SetActive(true);
        IsComplete = false;

        audioPlayer.PlayAfterDelay(finalAudio, 5f);

    }

    public override void UpdateStage()
    {
        if (IsClicked)
        {
            IsComplete = true;
        }

    }

    public override void Exit()
    {

        SceneManager.LoadScene(0);

    }

    public override string GetInstructionText()
    {
        return "View your results, and then exit the lab.";
    }

    public void ButtonClicked()
    {
        IsClicked = true;
    }

}
