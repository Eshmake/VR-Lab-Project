using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GrabDropAudio))]
public class AudioMuteZone : MonoBehaviour
{
    [Tooltip("List of trigger colliders that define quiet zones.")]
    public List<Collider> muteZones = new List<Collider>();

    private GrabDropAudio grabDropAudio;
    private int muteZoneCount = 0;

    void Awake()
    {
        grabDropAudio = GetComponent<GrabDropAudio>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (muteZones.Contains(other))
        {
            muteZoneCount++;
            UpdateMuteState();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (muteZones.Contains(other))
        {
            muteZoneCount = Mathf.Max(0, muteZoneCount - 1);
            UpdateMuteState();
        }
    }

    private void UpdateMuteState()
    {
        bool shouldMute = muteZoneCount > 0;

        if (grabDropAudio.grabSound != null)
            grabDropAudio.grabSound.mute = shouldMute;

        if (grabDropAudio.dropSound != null)
            grabDropAudio.dropSound.mute = shouldMute;
    }
}
