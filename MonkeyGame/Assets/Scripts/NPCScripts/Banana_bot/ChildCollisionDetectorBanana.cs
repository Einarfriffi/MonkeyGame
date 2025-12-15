using UnityEngine;

public class ChildCollisionDetectorBanana : MonoBehaviour
{
    [Header("Parent Script")]
    [SerializeField] BananaBotMovements parent_script;

    [Header("what the script is attached to")]
    [SerializeField] bool weakSpot;
    [SerializeField] bool Shield;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("bullet"))
        {
            if (weakSpot)
            {
                parent_script.WeakSpotHit(other);
            }
            if (Shield)
            {
                parent_script.ShieldtHit(other);
            }
        }
    }
}