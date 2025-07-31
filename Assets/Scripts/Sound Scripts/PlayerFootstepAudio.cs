using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudioOnPlayerMotion : MonoBehaviour
{
    public Transform playerRoot; // XR Origin or camera rig
    public float movementThreshold = 0.01f;  // Min movement to count as walking
    public float checkInterval = 0.1f;       // Frequency of checks (for performance)

    private AudioSource footstepAudio;
    private Vector3 lastPosition;
    private float timeSinceLastCheck = 0f;

    void Start()
    {
        footstepAudio = GetComponent<AudioSource>();
        if (playerRoot == null)
            playerRoot = Camera.main.transform.parent; // fallback
        lastPosition = playerRoot.position;
    }

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            Vector3 currentPos = playerRoot.position;
            Vector3 flatDelta = new Vector3(currentPos.x - lastPosition.x, 0, currentPos.z - lastPosition.z);
            float movement = flatDelta.magnitude;

            bool isMoving = movement > movementThreshold;

            if (isMoving && !footstepAudio.isPlaying)
                footstepAudio.Play();
            else if (!isMoving && footstepAudio.isPlaying)
                footstepAudio.Stop();

            lastPosition = currentPos;
            timeSinceLastCheck = 0f;
        }
    }
}
