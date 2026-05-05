using UnityEngine;
using UnityEngine.InputSystem;

public class FlatScreenPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    // Lowered default sensitivity since InputSystem mouse delta is usually larger than legacy GetAxis
    public float mouseSensitivity = 0.1f; 
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalLookRotation = 0f;
    private float gravityVelocity = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        LookAround();
        MovePlayer();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LookAround()
    {
        if (cameraTransform == null || Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    void MovePlayer()
    {
        if (Keyboard.current == null) return;

        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.rightArrowKey.isPressed) z += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) z -= 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;

        // Normalize to prevent faster diagonal movement
        Vector2 inputDir = new Vector2(x, z);
        if (inputDir.magnitude > 1f) inputDir.Normalize();

        Vector3 horizontalMove = transform.right * inputDir.x + transform.forward * inputDir.y;

        if (controller.isGrounded && gravityVelocity < 0f)
        {
            gravityVelocity = -1f;
        }

        gravityVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 verticalMove = Vector3.up * gravityVelocity;

        controller.Move((horizontalMove * moveSpeed + verticalMove) * Time.deltaTime);
    }
}
