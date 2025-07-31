using UnityEngine;
using System.Collections;

public class UIButtonSound : MonoBehaviour
{
    public AudioSource clickAudio;

    public void PlayClick()
    {
        if (clickAudio != null)
        {
            clickAudio.Play();
            StartCoroutine(Delay());
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.0f);
    }
}
