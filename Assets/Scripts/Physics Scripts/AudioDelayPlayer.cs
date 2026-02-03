using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelayPlayer : MonoBehaviour
{
    private StageAudioScope scope;
    private readonly List<Coroutine> active = new();

    public void SetScope(StageAudioScope newScope)
    {
        scope = newScope;
    }

    public void PlayAfterDelay(AudioSource src, float delaySeconds)
    {
        if (!src || scope == null) return;
        active.Add(StartCoroutine(PlayDelayed(src, delaySeconds)));
    }

    public void CancelAllDelayed()
    {
        foreach (var c in active)
            if (c != null) StopCoroutine(c);

        active.Clear();
    }

    IEnumerator PlayDelayed(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!src) yield break;

        scope.Register(src);
        src.Play();
    }
}

