using UnityEngine;

public class SinkHandleWaterController : MonoBehaviour
{
    [Header("References")]
    public HingeJoint handleJoint;
    public ParticleSystem waterParticleSystem;
    public AudioSource waterAudio;

    [Header("Settings")]
    public float closedAngle = 0f;
    public float activationThreshold = 5f; // degrees from closed before it turns on

    private bool waterActive = false;

    void Update()
    {
        if (handleJoint == null) return;

        float currentAngle = handleJoint.angle;
        float delta = Mathf.Abs(currentAngle - closedAngle);

        bool shouldActivate = delta > activationThreshold;

        if (shouldActivate && !waterActive)
        {
            TurnOnWater();
        }
        else if (!shouldActivate && waterActive)
        {
            TurnOffWater();
        }
    }

    private void TurnOnWater()
    {
        if (waterParticleSystem != null && !waterParticleSystem.isPlaying)
            waterParticleSystem.Play();

        if (waterAudio != null && !waterAudio.isPlaying)
            waterAudio.Play();

        waterActive = true;
    }

    private void TurnOffWater()
    {
        if (waterParticleSystem != null && waterParticleSystem.isPlaying)
            waterParticleSystem.Stop();

        if (waterAudio != null && waterAudio.isPlaying)
            waterAudio.Stop();

        waterActive = false;
    }
}
