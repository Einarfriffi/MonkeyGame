using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class missile_controller : MonoBehaviour
{
    [Header("Missile Settings")]
    public float acceleration = 5f;   // how fast it speeds up
    public float maxSpeed = 20f;
    public float maxRange = 10f;
    public GameObject Explosion;

    private Rigidbody2D rb;
    private Vector2 startPos;

    //private Vector2 original_pos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody component attached to this GameObject
        //original_pos = transform.position;
    }

    public void Init(float maxSpeed_, float acceleration_, float maxRange_ = 10f)
    {
        maxSpeed = maxSpeed_;
        acceleration = acceleration_;
        maxRange = maxRange_;
        startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Add velocity in the missile's forward direction (right side)
        rb.linearVelocity += acceleration * Time.fixedDeltaTime * (Vector2)transform.right;

        // Limit speed
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        if (Vector2.Distance(transform.position, startPos) >= maxRange)
        {
            Destroy(this.gameObject);
            Instantiate(Explosion, transform.position, Quaternion.identity);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if the missile collides with the player
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
            Instantiate(Explosion, transform.position, Quaternion.identity);
        }
        else // anything else 
        {
            //Destroy(collision.gameObject);
            Destroy(this.gameObject);
            Instantiate(Explosion, transform.position, Quaternion.identity);
        }
    }
}
