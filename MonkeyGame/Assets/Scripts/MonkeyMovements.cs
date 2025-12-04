using UnityEngine;
using UnityEngine.InputSystem;

public class MonkeyMovements : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Jumps")]
    [SerializeField] private int maxJumps = 2; 

    private GroundController _groundController;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _moveInput;
    private event System.Action _jumpPressed;
    private bool _jumpTriggered;

    private int _jumpsRemaining;
    private bool _wasGrounded;

    private void Start()
    {
        _groundController = GetComponent<GroundController>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _jumpPressed += JumpButtonPressed;

        _jumpsRemaining = maxJumps;
    }

    private void OnDestroy()
    {
        _jumpPressed -= JumpButtonPressed;
    }

    private void Update()
    {
        bool isGrounded = _groundController != null && _groundController.IsGrounded;

        if (isGrounded && !_wasGrounded)
        {
            _jumpsRemaining = maxJumps;
        }

        _wasGrounded = isGrounded;

        Vector2 velocity = _rigidbody2D.linearVelocity;
        velocity.x = _moveInput.x * speed;

        if (_jumpTriggered && _jumpsRemaining > 0)
        {
            velocity.y = jumpForce;
            _jumpsRemaining--;
            _jumpTriggered = false;
        }

        _rigidbody2D.linearVelocity = velocity;
    }

    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            _jumpPressed?.Invoke();
        }
    }

    private void JumpButtonPressed()
    {
        if (_jumpsRemaining > 0)
        {
            _jumpTriggered = true;
        }
    }
}
