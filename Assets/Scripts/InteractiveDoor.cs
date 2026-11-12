using UnityEngine;

public class InteractiveDoor : MonoBehaviour
{
    [Header("Door References")]
    public Transform doorPanel;

    [Header("Door Movement")]
    public float openAngle = 100f;
    public float rotationSpeed = 180f;

    public bool IsOpen { get; private set; }
    public bool HasBeenOpened { get; private set; }

    private Quaternion closedRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        closedRotation = transform.localRotation;
        targetRotation = closedRotation;

        if (doorPanel == null)
            Debug.LogWarning($"{name}: Door panel reference is missing.", this);
    }

    private void Update()
    {
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            targetRotation,
            Mathf.Max(0f, rotationSpeed) * Time.deltaTime
        );
    }

    public void Toggle(Transform player)
    {
        if (player == null || doorPanel == null)
            return;

        if (IsOpen)
        {
            IsOpen = false;
            targetRotation = closedRotation;
            return;
        }

        float side = Vector3.Dot(player.position - transform.position, transform.forward);
        float direction = side >= 0f ? 1f : -1f;

        IsOpen = true;
        HasBeenOpened = true;
        targetRotation = closedRotation * Quaternion.Euler(0f, direction * openAngle, 0f);
    }
}
