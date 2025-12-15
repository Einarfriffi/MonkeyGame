using UnityEngine;

public class SunFollowAnchor : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform anchor;   // place this in the world (not camera)

    [Range(0f, 1f)]
    [SerializeField] private float followScale = 0.2f;  // 0 = no move, 1 = same as player

    [Header("Sun movement zone around anchor")]
    [SerializeField] private float minOffsetX = -8f;
    [SerializeField] private float maxOffsetX =  8f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 5f;

    private float fixedY, fixedZ;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (!player || !anchor) return;

        // scaled follow: sun moves only a fraction of player's distance from the anchor
        float desiredX = anchor.position.x + (player.position.x - anchor.position.x) * followScale;

        // clamp into a designated area around the anchor
        float clampedX = Mathf.Clamp(desiredX, anchor.position.x + minOffsetX, anchor.position.x + maxOffsetX);

        Vector3 targetPos = new Vector3(clampedX, fixedY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
