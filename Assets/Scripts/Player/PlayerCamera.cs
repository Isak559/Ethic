
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;

    [Header("Camera Settings")]
    public float mouseSensitivity = 0.1f;
    public float minLookAngle = -85f;
    public float maxLookAngle = 85f;

    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _xRotation = transform.localEulerAngles.x;
        _yRotation = playerBody.eulerAngles.y;
    }

    private void Update()
    {
        if (Mouse.current == null || playerBody == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        // Vertical camera rotation
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(
            _xRotation,
            minLookAngle,
            maxLookAngle
        );

        // Horizontal player rotation
        _yRotation += mouseX;

        // Apply camera rotation
        transform.localRotation =
            Quaternion.Euler(_xRotation, 0f, 0f);

        // Apply player rotation
        playerBody.rotation =
            Quaternion.Euler(0f, _yRotation, 0f);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
