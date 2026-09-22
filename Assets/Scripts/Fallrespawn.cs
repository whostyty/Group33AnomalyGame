using UnityEngine;

public class FallRespawn : MonoBehaviour
{
    public Transform spawnPoint;
    public float fallThreshold = -20f;

    private CharacterController controller;
    private FPController fpController;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        fpController = GetComponent<FPController>();

        if (spawnPoint == null)
            Debug.LogError($"{name}: FallRespawn has no Spawn Point assigned.", this);
    }

    private void Update()
    {
        if (transform.position.y < fallThreshold)
            Respawn();
    }

    private void Respawn()
    {
        if (spawnPoint == null) return;

        if (controller != null)
            controller.enabled = false;

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        if (controller != null)
            controller.enabled = true;

        if (fpController != null)
            fpController.ResetVelocity();
    }
}