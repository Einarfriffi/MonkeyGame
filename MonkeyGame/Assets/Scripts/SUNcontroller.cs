using UnityEngine;

public class SUNcontroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("FIX NO WORK")]
    public Transform player;
    public Rigidbody2D rb;
    public float ratio;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        rb.linearVelocity += new Vector2(player.position.x / ratio, 0);
    }
}
