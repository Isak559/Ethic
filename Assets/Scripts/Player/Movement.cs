
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector2 _movement;
    private bool _isGrounded;

    [Header("Movement")]
    public float _speed = 5f;

    [Header("Jumping")]
    public float _jumpForce = 7f;
    public LayerMask _groundLayer;
    public Transform _groundCheck;
    public float _groundCheckRadius = 0.2f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            _movement = Vector2.zero;
        }
    }

    private void Update()
    {
        CheckGrounded();

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            _isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Keep movement horizontal
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Calculate movement direction
        Vector3 movement =
            forward * _movement.y +
            right * _movement.x;

        movement = Vector3.ClampMagnitude(movement, 1f);

        // Stop horizontal movement immediately
        Vector3 velocity = _rigidbody.linearVelocity;

        velocity.x = movement.x * _speed;
        velocity.z = movement.z * _speed;

        _rigidbody.linearVelocity = velocity;
    }

    private void CheckGrounded()
    {
        if (_groundCheck == null)
        {
            _isGrounded = false;
            return;
        }

        _isGrounded = Physics.CheckSphere(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    private void Jump()
    {
        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.y = 0f;

        _rigidbody.linearVelocity = velocity;

        _rigidbody.AddForce(
            Vector3.up * _jumpForce,
            ForceMode.Impulse
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            _groundCheck.position,
            _groundCheckRadius
        );
    }
}
