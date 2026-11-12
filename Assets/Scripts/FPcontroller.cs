using UnityEngine;
using UnityEngine.InputSystem;

public class FPController : MonoBehaviour, Controls.IPlayerActions
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float terminalVelocity = -50f;

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 0.0685f;
    public float gamepadLookSpeed = 129.5f;
    public float lookSmoothingHalfLife = 0.025f;
    public float verticalLookLimit = 90f;
    public float initialVerticalLook = 0f;

    private CharacterController controller;
    private Controls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 smoothedLook;
    private Vector3 velocity;
    private float verticalRotation;
    private bool skipNextLookFrame;
    private bool usingGamepadLook;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError($"{name}: FPController requires a CharacterController component.", this);

        if (cameraTransform == null)
            Debug.LogError($"{name}: FPController is missing a cameraTransform reference.", this);

        controls = new Controls();
        controls.Player.SetCallbacks(this);
        RefreshLookSettings();
        HallPassBrightness.EnsureExists();

        verticalRotation = initialVerticalLook;
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        LockCursor();
    }

    private void OnEnable()
    {
        RefreshLookSettings();
        controls?.Player.Enable();
        ClearLookInput();
    }

    private void OnDisable()
    {
        controls?.Player.Disable();
        ClearLookInput();
    }

    private void OnDestroy()
    {
        controls?.Dispose();
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        usingGamepadLook = context.control != null && context.control.device is Gamepad;
    }

    public Vector2 MoveInput => moveInput;
    public Vector2 LookInput => lookInput;

    public void RefreshLookSettings()
    {
        lookSensitivity = HallPassSettings.MouseSensitivity;
        gamepadLookSpeed = HallPassSettings.GamepadLookSpeed;
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    public void TeleportTo(Vector3 position, Quaternion rotation)
    {
        if (controller != null)
            controller.enabled = false;

        transform.SetPositionAndRotation(position, rotation);

        if (controller != null)
            controller.enabled = true;

        velocity = Vector3.zero;
        verticalRotation = initialVerticalLook;
        ClearLookInput();
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    public void HandleMovement()
    {
        if (controller == null)
            return;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        else
        {
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, terminalVelocity);
        }

        controller.Move(velocity * Time.deltaTime);
    }

    public void HandleLook()
    {
        if (cameraTransform == null)
            return;

        if (skipNextLookFrame)
        {
            skipNextLookFrame = false;
            lookInput = Vector2.zero;
            smoothedLook = Vector2.zero;
            return;
        }

        float deltaTime = Time.unscaledDeltaTime;
        if (deltaTime <= 0f)
            return;

        Vector2 targetDelta = usingGamepadLook
            ? lookInput * gamepadLookSpeed * deltaTime
            : lookInput * lookSensitivity;

        float alpha = lookSmoothingHalfLife <= 0f
            ? 1f
            : 1f - Mathf.Exp(-0.69314718f * deltaTime / Mathf.Max(0.001f, lookSmoothingHalfLife));

        smoothedLook = Vector2.Lerp(smoothedLook, targetDelta, alpha);
        if (targetDelta.sqrMagnitude < 0.000001f && smoothedLook.sqrMagnitude < 0.000001f)
            smoothedLook = Vector2.zero;

        verticalRotation = Mathf.Clamp(verticalRotation - smoothedLook.y, -verticalLookLimit, verticalLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * smoothedLook.x);
    }

    private void ClearLookInput()
    {
        lookInput = Vector2.zero;
        smoothedLook = Vector2.zero;
        skipNextLookFrame = true;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClearLookInput();
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
