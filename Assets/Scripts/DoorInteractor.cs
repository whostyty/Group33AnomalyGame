using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class DoorInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3.2f;
    public LayerMask interactionLayers = ~0;

    [Header("Interaction Prompt")]
    public bool showInteractionPrompt = true;

    private FPController player;
    private InteractiveDoor targetedDoor;

    private void Awake()
    {
        player = GetComponent<FPController>();
    }

    private void Update()
    {
        targetedDoor = null;

        if (player == null || !player.isActiveAndEnabled || player.cameraTransform == null || Time.timeScale <= 0f)
            return;

        Ray ray = new Ray(player.cameraTransform.position, player.cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayers, QueryTriggerInteraction.Ignore))
            targetedDoor = hit.collider.GetComponentInParent<InteractiveDoor>();

        if (targetedDoor != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            targetedDoor.Toggle(transform);
    }


    private void OnGUI()
    {
        if (!showInteractionPrompt || targetedDoor == null || player == null || !player.isActiveAndEnabled || Time.timeScale <= 0f)
            return;

        string action = targetedDoor.IsOpen ? "Close door" : "Open door";

        float width = 385f;
        float height = 80f;

        Rect area = new Rect(
            (Screen.width - width) * 0.5f,
            Screen.height - 145f,
            width,
            height
        );

        GUIStyle promptStyle = new GUIStyle(GUI.skin.box);
        promptStyle.fontSize = 28;
        promptStyle.fontStyle = FontStyle.Bold;
        promptStyle.alignment = TextAnchor.MiddleCenter;

        GUI.Box(area, "[E]  " + action, promptStyle);
    }

}
