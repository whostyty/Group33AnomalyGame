using UnityEngine;

public class InteractiveDoor : MonoBehaviour
{
    [Header("Door References")]
    public Transform doorPanel;
    public Transform rotationPivot;

    [Header("Door Movement")]
    [Range(0f, 180f)] public float openAngle = 100f;
    public float rotationSpeed = 180f;
    public bool openAwayFromPlayer = true;
    public bool startsOpen = false;
    public bool canInteract = true;

    [Header("Prompt Text")]
    public string openPrompt = "Open door";
    public string closePrompt = "Close door";

    public bool IsOpen { get; private set; }
    public bool HasBeenOpened { get; private set; }

    private Transform hinge;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        hinge = rotationPivot != null ? rotationPivot : transform;
        closedRotation = hinge.localRotation;
        targetRotation = closedRotation;
        IsOpen = startsOpen;
        if (startsOpen)
        {
            targetRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
            hinge.localRotation = targetRotation;
            HasBeenOpened = true;
        }
        if (doorPanel == null)
            Debug.LogWarning($"{name}: Assign a door panel Transform in the Inspector.", this);
    }

    private void Update()
    {
        if (hinge == null) return;
        hinge.localRotation = Quaternion.RotateTowards(hinge.localRotation, targetRotation, Mathf.Max(0f, rotationSpeed) * Time.deltaTime);
    }

    public void Toggle(Transform player)
    {
        if (!canInteract || hinge == null || player == null || doorPanel == null) return;
        if (IsOpen)
        {
            IsOpen = false;
            targetRotation = closedRotation;
            return;
        }
        float direction = 1f;
        if (openAwayFromPlayer)
            direction = Vector3.Dot(player.position - hinge.position, hinge.forward) >= 0f ? 1f : -1f;
        IsOpen = true;
        HasBeenOpened = true;
        targetRotation = closedRotation * Quaternion.Euler(0f, direction * openAngle, 0f);
    }
}
