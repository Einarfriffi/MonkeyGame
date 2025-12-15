using UnityEngine;

public class FlagMotionButtons : MonoBehaviour
{
    [Header("Rotation")]
    public float swayAngle = 5f;
    public float swaySpeed = 2f;

    [Header("Stretch")]
    public float stretchAmount = 0.03f;
    public float stretchSpeed = 3f;

    // Random offsets per instance
    float swaySpeedRnd;
    float stretchSpeedRnd;
    float timeOffset;

    RectTransform rect;
    Vector3 baseScale;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        baseScale = rect.localScale;

        // Randomize speed slightly (natural variation)
        swaySpeedRnd = Random.Range(0.8f, 1.2f);
        stretchSpeedRnd = Random.Range(0.8f, 1.2f);

        // Randomize the phase so each object starts differently
        timeOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float t = Time.time + timeOffset;

        // Randomized sway
        float angle = Mathf.Sin(t * swaySpeed * swaySpeedRnd) * swayAngle;
        rect.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Randomized stretch pulse
        float stretch = 1f + Mathf.Sin(t * stretchSpeed * stretchSpeedRnd) * stretchAmount;
        rect.localScale = new Vector3(baseScale.x * stretch, baseScale.y, baseScale.z);
    }
}
