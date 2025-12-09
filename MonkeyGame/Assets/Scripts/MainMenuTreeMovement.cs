using UnityEngine;

public class TreeWindSway : MonoBehaviour
{
    [Header("Base Settings")]
    public float swayAngle = 3f;     // How far it leans
    public float swaySpeed = 1f;     // Wind sway speed
    public float scaleAmount = 0.02f;
    public float scaleSpeed = 1.5f;

    // Random multipliers/offsets (internal)
    float swaySpeedRnd;
    float scaleSpeedRnd;
    float timeOffset;

    RectTransform rect;
    Vector3 baseScale;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        baseScale = rect.localScale;

        // Create natural variation
        swaySpeedRnd = Random.Range(0.8f, 1.2f);
        scaleSpeedRnd = Random.Range(0.8f, 1.2f);

        // Phase offset so they don't start synchronized
        timeOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float t = Time.time + timeOffset;

        // Rotation sway
        float angle = Mathf.Sin(t * swaySpeed * swaySpeedRnd) * swayAngle;
        rect.localRotation = Quaternion.Euler(0, 0, angle);

        // Tiny scale breathing
        float s = 1f + Mathf.Sin(t * scaleSpeed * scaleSpeedRnd) * scaleAmount;
        rect.localScale = new Vector3(baseScale.x * s, baseScale.y * s, baseScale.z);
    }
}
