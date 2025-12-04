using UnityEngine;
using UnityEngine.InputSystem;

public class MonkeyMovements : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Jumps")]
    [SerializeField] private int maxJumps = 2;

    // animator
    [SerializeField] Animator animator;
    // get the sprite child
    [SerializeField] SpriteRenderer _monkey_sprite;

    [SerializeField] float monkey_death_time = 3f;

    private GroundController _groundController;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _moveInput;
    private event System.Action _jumpPressed;
    private bool _jumpTriggered;

    private int _jumpsRemaining;
    private bool _wasGrounded;

    private bool dead = false;

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

    private void FixedUpdate()
    {
        if (!dead)
        {
            bool isGrounded = _groundController != null && _groundController.IsGrounded;

            if (isGrounded && !_wasGrounded)
            {
                _jumpsRemaining = maxJumps;
                animator.SetBool("is_jumping", false);
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

            // sets the speed to the velocity of the player for animator
            animator.SetFloat("player_speed", Mathf.Abs(velocity.x));
            // set the falling speed of the player for animator
            animator.SetFloat("y_movment", Mathf.Sign(velocity.y));
            // checks if player is going left or right
            if (_rigidbody2D.linearVelocity.x > 0.01f)
            {
                _monkey_sprite.flipX = false;
            }
            else if (_rigidbody2D.linearVelocity.x < -0.01f)
            {
                _monkey_sprite.flipX = true;
            }


            _rigidbody2D.linearVelocity = velocity;

        }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazards"))
        {
            dead = true;
            animator.SetBool("is_dead", true);

            // stop movement
            _rigidbody2D.linearVelocity = Vector2.zero;
            _moveInput = Vector2.zero;

            GetComponent<PlayerInput>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void JumpButtonPressed()
    {
        if (_jumpsRemaining > 0)
        {
            _jumpTriggered = true;
            animator.SetBool("is_jumping", true);
        }
    }
}
