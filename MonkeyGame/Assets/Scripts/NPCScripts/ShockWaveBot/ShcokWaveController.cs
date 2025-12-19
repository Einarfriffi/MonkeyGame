using UnityEngine;

public class ShcokWaveController : MonoBehaviour
{
    [Header("Shock Wave Settings")]
    public float acceleration = 5f;
    public float maxSpeed = 20f;
    public float maxRange = 10f;
    public float direction;
    public float launchForce;
    [SerializeField] private LayerMask collisionLayers;

    private Rigidbody2D rb;
    private Vector2 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody component attached to this GameObject
        //original_pos = transform.position;
    }

    public void Init(float maxSpeed_, float acceleration_, float maxRange_ = 10f, float direction_ = 1f, float launchForce_ = 5f)
    {
        maxSpeed = maxSpeed_;
        acceleration = acceleration_;
        maxRange = maxRange_;
        startPos = transform.position;
        direction = direction_;
        launchForce = launchForce_;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Add velocity in the missile's forward direction (right side)
        rb.linearVelocity += direction * acceleration * Time.fixedDeltaTime * (Vector2)transform.right;

        // Limit speed
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        if (Vector2.Distance(transform.position, startPos) >= maxRange)
        {
            Destroy(this.gameObject);
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Debug.Log("meow");
                // Optional: reset vertical velocity for consistency
                //rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, launchForce);

                // Apply upward force
                //rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
            }
        }
    }
}
