using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class missile_controller : MonoBehaviour
{
    private Rigidbody2D rb;
    public float acceleration = 5f;   // how fast it speeds up
    public float maxSpeed = 20f;

    public float maxRange = 10f;

    //private Vector2 original_pos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody component attached to this GameObject
        //original_pos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Add velocity in the missile's forward direction (right side)
        rb.linearVelocity += (Vector2)transform.right * acceleration * Time.fixedDeltaTime;

        // Limit speed
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        if (transform.position.x >= maxRange | transform.position.x <= -maxRange)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if the missile collides with the player
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);

        }
        else if (other.gameObject.CompareTag("Hazards"))
        {

        }
        else // anything else 
        {
            //Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
