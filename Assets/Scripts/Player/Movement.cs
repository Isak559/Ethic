
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float _speed = 5f;
    public float _sprintSpeed = 9f;
    public float _jumpForce = 7f;

    private Rigidbody _rigidbody;
    private Vector2 _movement;

    [Header("Ground Check")]
    public LayerMask _groundLayer;
    public Transform _groundCheck;
    public float _groundCheckRadius = 0.2f;

    private bool _isGrounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            _movement = Vector2.zero;
        }
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer
        );

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            _isGrounded)
        {
            _rigidbody.AddForce(
                Vector3.up * _jumpForce,
                ForceMode.Impulse
            );
        }
    }

    private void FixedUpdate()
    {
        // Check if sprint key is held
        bool isSprinting = Keyboard.current != null &&
                           Keyboard.current.leftShiftKey.isPressed;

        // Choose movement speed
        float currentSpeed = isSprinting ? _sprintSpeed : _speed;

        // Get player-relative directions
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Calculate movement direction
        Vector3 movementDirection =
            forward * _movement.y +
            right * _movement.x;

        // Apply movement
        Vector3 velocity = movementDirection * currentSpeed;

        _rigidbody.linearVelocity = new Vector3(
            velocity.x,
            _rigidbody.linearVelocity.y,
            velocity.z
        );
    }
}
