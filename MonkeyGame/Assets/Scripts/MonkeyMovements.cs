using UnityEngine;
using UnityEngine.InputSystem;

public class MonkeyMovements : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Jumps")]
    [SerializeField] private int maxJumps = 2;

    [Header("Vine")]
    [SerializeField] private string vineTag = "Vine";      // Tag used on vine segments
    [SerializeField] private float vineSwingForce = 10f;   // Left/right push while hanging
    [SerializeField] private float vineReattachDelay = 0.25f; // cooldown before we can stick again

    // Animator / visuals
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer _monkey_sprite;

    private GroundController _groundController;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _moveInput;
    private event System.Action _jumpPressed;
    private bool _jumpTriggered;

    private int _jumpsRemaining;
    private bool _wasGrounded;

    private bool dead = false;

    // Vine state
    private bool _attachedToVine;
    private HingeJoint2D _vineJoint;
    private bool _canAttachToVine = true;
    private float _vineCooldownTimer;

    private void Start()
    {
        _groundController = GetComponent<GroundController>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _jumpPressed += JumpButtonPressed;

        _jumpsRemaining = maxJumps;

        // Prepare the hinge we use to attach to vines (disabled by default)
        _vineJoint = gameObject.AddComponent<HingeJoint2D>();
        _vineJoint.enabled = false;
        _vineJoint.autoConfigureConnectedAnchor = true; // let Unity choose nice anchors
    }

    private void OnDestroy()
    {
        _jumpPressed -= JumpButtonPressed;
    }

    private void Update()
    {
        // Handle vine re-attach cooldown
        if (!_canAttachToVine)
        {
            _vineCooldownTimer -= Time.deltaTime;
            if (_vineCooldownTimer <= 0f)
            {
                _canAttachToVine = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (dead) return;

        // === HANGING ON VINE ===
        if (_attachedToVine)
        {
            // Use left/right input to pump the swing a bit
            _rigidbody2D.AddForce(
                new Vector2(_moveInput.x * vineSwingForce, 0f),
                ForceMode2D.Force
            );

            // Jump while on vine -> detach + jump impulse
            if (_jumpTriggered)
            {
                DetachFromVine();

                Vector2 vel = _rigidbody2D.linearVelocity;
                vel.y = jumpForce;
                _rigidbody2D.linearVelocity = vel;

                animator.SetBool("is_jumping", true);
                _jumpTriggered = false;
            }

            // Update animator while hanging
            animator.SetFloat("player_speed", Mathf.Abs(_rigidbody2D.linearVelocity.x));
            animator.SetFloat("y_movment", Mathf.Sign(_rigidbody2D.linearVelocity.y));

            if (_rigidbody2D.linearVelocity.x > 0.01f)
                _monkey_sprite.flipX = false;
            else if (_rigidbody2D.linearVelocity.x < -0.01f)
                _monkey_sprite.flipX = true;

            return; // important: no normal ground movement this frame
        }

        // === NORMAL GROUND / AIR MOVEMENT ===
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

        animator.SetFloat("player_speed", Mathf.Abs(velocity.x));
        animator.SetFloat("y_movment", Mathf.Sign(velocity.y));

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

            _rigidbody2D.linearVelocity = Vector2.zero;
            _moveInput = Vector2.zero;

            GetComponent<PlayerInput>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GameManager.Instance.ShowDeathScreen();
        }

        if (other.CompareTag("Win"))
        {
            GameManager.Instance.LevelWon();
        }
    }

    private void JumpButtonPressed()
    {
        // While on vine, jump = let go of vine
        if (_attachedToVine)
        {
            _jumpTriggered = true;
            return;
        }

        // Normal jumping
        if (_jumpsRemaining > 0)
        {
            _jumpTriggered = true;
            animator.SetBool("is_jumping", true);
        }
    }

    // ===== VINE ATTACH / DETACH =====

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_attachedToVine) return;
        if (!_canAttachToVine) return;    
        if (!collision.collider.CompareTag(vineTag)) return;
        if (collision.rigidbody == null) return;

        AttachToVine(collision.rigidbody);
    }

    private void AttachToVine(Rigidbody2D vineBody)
    {
        _attachedToVine = true;

        _rigidbody2D.linearVelocity = Vector2.zero;

        _vineJoint.connectedBody = vineBody;
        _vineJoint.enabled = true;  
    }

    private void DetachFromVine()
    {
        _attachedToVine = false;
        _vineJoint.enabled = false;
        _vineJoint.connectedBody = null;

        // short delay so we don't re-stick immediately
        _canAttachToVine = false;
        _vineCooldownTimer = vineReattachDelay;
    }
}
