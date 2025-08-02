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

    private bool enteredLab = false;

    public override void Enter()
    {
        introPanel.SetActive(true);
        door.SetActive(false);
        lights.SetActive(false);


        enteredLab = false;
        IsComplete = false;

        StartCoroutine(PlayAfterDelay(2f, introAudio));

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
            StartCoroutine(PlayAfterDelay(1f, ambience));

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

    private IEnumerator PlayAfterDelay(float delay, AudioSource audio)
    {
        yield return new WaitForSeconds(delay);

        if (audio != null)
        {
            audio.Play();
        }
    }
}