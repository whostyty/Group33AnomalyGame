using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class DoorInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3.2f;
    public LayerMask interactionLayers = ~0;
    public bool enableGamepadInteraction = true;

    [Header("Interaction Prompt")]
    public bool showInteractionPrompt = true;
    public string keyboardButtonLabel = "E";
    public string controllerButtonLabel = "A";
    public int promptFontSize = 28;
    public float promptWidth = 385f;
    public float promptHeight = 80f;
    public float promptBottomOffset = 65f;

    private FPController player;
    private InteractiveDoor targetedDoor;
    private bool usingController;

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
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Max(0f, interactionDistance), interactionLayers, QueryTriggerInteraction.Ignore))
            targetedDoor = hit.collider.GetComponentInParent<InteractiveDoor>();

        if (targetedDoor == null || !targetedDoor.canInteract) return;
        bool keyboardPressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
        bool gamepadPressed = enableGamepadInteraction && Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;
        if (keyboardPressed || gamepadPressed)
        {
            usingController = gamepadPressed;
            targetedDoor.Toggle(transform);
        }
    }

    private void OnGUI()
    {
        if (!showInteractionPrompt || targetedDoor == null || !targetedDoor.canInteract || player == null || !player.isActiveAndEnabled || Time.timeScale <= 0f)
            return;

        string buttonName = usingController ? controllerButtonLabel : keyboardButtonLabel;
        string action = targetedDoor.IsOpen ? targetedDoor.closePrompt : targetedDoor.openPrompt;
        float w = Mathf.Clamp(promptWidth, 100f, Screen.width);
        float h = Mathf.Clamp(promptHeight, 28f, Screen.height);
        float x = (Screen.width - w) * 0.5f;
        float y = Screen.height - Mathf.Max(0f, promptBottomOffset) - h;
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = Mathf.Max(10, promptFontSize);
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(x, y, w, h), "[" + buttonName + "]  " + action, style);
    }
}
