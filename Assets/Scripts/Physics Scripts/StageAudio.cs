using UnityEngine;

public class StageAudio : MonoBehaviour
{
    [SerializeField] StageAudioScope scope;

    void Awake()
    {
        if (!scope) scope = GetComponentInParent<StageAudioScope>();
    }

    // For long/looping sounds you want to forcibly stop on stage end:
    public AudioSource PlayScoped(AudioSource src, bool loop = false)
    {
        if (!src) return null;

        src.loop = loop;
        scope?.Register(src);
        src.Play();
        return src;
    }

    public void StopScoped(AudioSource src)
    {
        if (!src) return;
        src.Stop();
        scope?.Unregister(src);
    }

    // For one-shots you *usually* don't need to register,
    // but you CAN if you want the scope to stop them too.
    public void PlayOneShotScoped(AudioSource src, AudioClip clip, float volume = 1f)
    {
        if (!src || !clip) return;

        scope?.Register(src);
        src.PlayOneShot(clip, volume);
        // note: we don't unregister here because we can’t easily know when the oneshot ends
        // (if you need perfect cleanup, see the “One-shot cleanup” note below)
    }
}
