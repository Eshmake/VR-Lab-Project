using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    [SerializeField] private float fallThreshold = -10f;  // Y-value that counts as "falling off"
    [SerializeField] private Transform respawnPoint;      // Where the player will be reset

    private Transform playerTransform;

    void Start()
    {
        playerTransform = transform;
    }

    void Update()
    {
        if (playerTransform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // Optionally reset velocity if using Rigidbody
        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        playerTransform.position = respawnPoint.position;
        playerTransform.rotation = respawnPoint.rotation;
    }
}
