using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionTime = 0.45f;
    void Start()
    {
        Destroy(gameObject, explosionTime); // match the animation length
    }
}
