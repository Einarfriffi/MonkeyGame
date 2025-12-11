using UnityEngine;

public class FlagMotion : MonoBehaviour
{
    [Header("Rotation")]
    public float swayAngle = 5f;
    public float swaySpeed = 2f;

    [Header("Stretch")]
    public float stretchAmount = 0.03f;
    public float stretchSpeed = 3f;

    RectTransform rect;
    Vector3 baseScale;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        baseScale = rect.localScale;
    }

    void Update()
    {
        float t = Time.time;

        // Rotate movement
        float angle = Mathf.Sin(t * swaySpeed) * swayAngle;
        rect.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Slight stretch movement
        float stretch = 1f + Mathf.Sin(t * stretchSpeed) * stretchAmount;
        rect.localScale = new Vector3(baseScale.x * stretch, baseScale.y, baseScale.z);
    }
}
