using UnityEngine;

public class bulletImpact : MonoBehaviour
{
    public float explosionTime = 0.45f;
    void Start()
    {
        Destroy(gameObject, explosionTime); // match the animation length
    }
}
