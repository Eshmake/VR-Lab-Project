using System.Collections;
using UnityEngine;

public class AudioDelayPlayer : MonoBehaviour
{
    /// <summary>
    /// Plays an AudioSource after a specified delay.
    /// </summary>
    public void PlayAfterDelay(AudioSource source, float delay)
    {
        if (source != null)
            StartCoroutine(PlayWithDelay(source, delay));
    }

    /// <summary>
    /// Stops an AudioSource after an optional delay.
    /// </summary>
    public void StopAfterDelay(AudioSource source, float delay = 0f)
    {
        if (source != null)
            StartCoroutine(StopWithDelay(source, delay));
    }

    private IEnumerator PlayWithDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Play();
    }

    private IEnumerator StopWithDelay(AudioSource source, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        source.Stop();
    }
}
