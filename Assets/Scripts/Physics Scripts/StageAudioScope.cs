using UnityEngine;
using System.Collections.Generic;


public class StageAudioScope : MonoBehaviour
{
    private readonly HashSet<AudioSource> _sources = new HashSet<AudioSource>();

    public void Register(AudioSource src)
    {
        if(src != null) _sources.Add(src);
    }

    public void Unregister(AudioSource src)
    {
        if (src != null) _sources.Remove(src);
    }

    public void StopAll()
    {
        foreach (var src in _sources)
        {
            if(!src) continue;
            else src.Stop();
        }

        _sources.Clear();
    }

}
