using UnityEngine;
using System.Collections;

public class IntroStage : StageBase
{

    public AudioSource introAudio;
    public AudioSource doorShut;
    public AudioSource ambience;

    public GameObject introPanel;
    public GameObject door;
    public GameObject lights;

    public AudioDelayPlayer audioPlayer;

    private bool enteredLab = false;

    public override void Enter()
    {
        introPanel.SetActive(true);
        door.SetActive(false);
        lights.SetActive(false);


        enteredLab = false;
        IsComplete = false;

        audioPlayer.PlayAfterDelay(introAudio, 2f);

    }

    public override void UpdateStage()
    {
        if (enteredLab && !IsComplete)
        {
            lights.SetActive(true);
            introPanel.SetActive(false);
            door.SetActive(true);
            IsComplete = true;
            doorShut.Play();

            audioPlayer.PlayAfterDelay(ambience, 2f);
        }

    }

    public override void Exit()
    {
      
    }

    public override string GetInstructionText()
    {
        return "Listen to the introduction and enter the lab.";
    }

    public void NotifyEntry()
    {
        enteredLab = true;
    }

}