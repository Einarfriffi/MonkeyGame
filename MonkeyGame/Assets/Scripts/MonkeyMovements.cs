using UnityEngine;
using UnityEngine.InputSystem;

public class MonkeyMovements : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private GroundController _groundController;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _moveInput;
    private event System.Action _jumpPressed;
    private bool _jumpTriggered;

    private void Start()
    {
        _groundController = GetComponent<GroundController>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _jumpPressed += JumpButtonPressed;
    }

    private void OnDestroy()
    {
        _jumpPressed -= JumpButtonPressed;
    }

    private void Update()
    {
        Vector2 velocity = _rigidbody2D.linearVelocity;
        velocity.x = _moveInput.x * speed;

        if (_jumpTriggered)
        {
            velocity.y = jumpForce;
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
        if (_groundController != null && _groundController.IsGrounded)
        {
            _jumpTriggered = true;
        }
    }
}
