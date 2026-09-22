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
    public float lookSensitivity = 0.1f;
    public float verticalLookLimit = 90f;
    public float initialVerticalLook = 0f;

    private CharacterController controller;
    private Controls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private bool skipNextLookFrame = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError($"{name}: FPController requires a CharacterController component.", this);

        if (cameraTransform == null)
            Debug.LogError($"{name}: FPController is missing a cameraTransform reference.", this);

        controls = new Controls();
        controls.Player.SetCallbacks(this);

        verticalRotation = initialVerticalLook;
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        LockCursor();
    }

    private void OnEnable()
    {
        controls?.Player.Enable();
    }

    private void OnDisable()
    {
        controls?.Player.Disable();
    }

    private void OnDestroy()
    {
        controls?.Dispose();
    }

    private void Update()
    {
        HandleCursorToggle();
        HandleMovement();
        HandleLook();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void HandleMovement()
    {
        if (controller == null) return;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, terminalVelocity);
        }

        controller.Move(velocity * Time.deltaTime);
    }

    public void HandleLook()
    {
        if (cameraTransform == null) return;

        if (skipNextLookFrame)
        {
            skipNextLookFrame = false;
            lookInput = Vector2.zero;
            return;
        }

        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCursorToggle()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                UnlockCursor();
            else
                LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lookInput = Vector2.zero;
        skipNextLookFrame = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}