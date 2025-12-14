using UnityEngine;

public class BrakeBlock : MonoBehaviour
{
    // tag bullet
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("bullet"))
        {
            Destroy(gameObject);
        }
    }
}
