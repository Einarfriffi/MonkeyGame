using UnityEngine;
using UnityEngine.Tilemaps;

public class BananaBotMovements : MonoBehaviour
{
    // Variable for path control and speed control
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;

    // iniate direction, body and renderer
    private int direction = 1;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Fetch Componenets
        rb = GetComponent<Rigidbody2D>();
        // lock body rotation
        rb.freezeRotation = true;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // get position
        Vector2 pos = rb.position;
        
        // movement
        pos.x += direction * speed * Time.fixedDeltaTime;

        // check bounds and change direction
        if (pos.x >= maxX)
        {
            pos.x = maxX;
            direction = -1;
        }
        else if (pos.x <= minX)
        {
            pos.x = minX;
            direction = 1;
        }

        // sprite direction flip
        if (sr != null)
        {
            sr.flipX = direction > 0;
        }

        rb.MovePosition(pos);
    }
}
