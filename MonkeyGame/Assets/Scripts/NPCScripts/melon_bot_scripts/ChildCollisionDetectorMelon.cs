using UnityEngine;

public class ChildCollisionDetectorMelon : MonoBehaviour
{
    [Header("Parent Script")]
    [SerializeField] melon_missile_launcher parent_script;

    [Header("what the script is attached to")]
    [SerializeField] bool weakSpot;
    [SerializeField] bool Shield;

    [Header("Sound Effects")]
    public AudioClip dyingSoundClip;
    public AudioClip shieldtHitClip;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("bullet"))
        {
            if (weakSpot)
            {
                parent_script.WeakSpotHit(other);

                // play death sound
                SFXManager.instance.PlaySoundEffect(dyingSoundClip, transform, 0.6f);
            }
            if (Shield)
            {
                parent_script.ShieldtHit(other);

                // play shield sound
                SFXManager.instance.PlaySoundEffect(shieldtHitClip, transform, 0.6f);
            }
        }
    }
}