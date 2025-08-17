using UnityEngine;

public class WaterRunDetector : MonoBehaviour
{
    [Tooltip("ParticleSystems that represent running water. Detection is true if ANY is playing/emitting.")]
    public ParticleSystem[] waterParticles;

    [Tooltip("Optional: audio sources that play with the water (not required).")]
    public AudioSource[] waterAudio;

    /// <summary>True if any particle is playing/emitting OR any audio is playing.</summary>
    public bool IsRunningNow()
    {
        // Particles
        if (waterParticles != null)
        {
            for (int i = 0; i < waterParticles.Length; i++)
            {
                var ps = waterParticles[i];
                if (ps == null) continue;

                // Either check is sufficient; 'isEmitting' can be more responsive with sub-emitters
                if (ps.isEmitting || ps.isPlaying)
                    return true;
            }
        }

        // Audio (optional)
        if (waterAudio != null)
        {
            for (int i = 0; i < waterAudio.Length; i++)
            {
                var a = waterAudio[i];
                if (a != null && a.isPlaying)
                    return true;
            }
        }

        return false;
    }
}
