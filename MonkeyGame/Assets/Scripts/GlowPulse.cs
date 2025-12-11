using UnityEngine;
using UnityEngine.UI;

public class GlowPulse : MonoBehaviour
{
    public float pulseAmount = 0.3f; // 0–1, how strong the pulse
    public float pulseSpeed = 2f;

    Image img;
    Color baseColor;

    void Awake()
    {
        img = GetComponent<Image>();
        baseColor = img.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // 0–1
        float a = Mathf.Lerp(baseColor.a * (1f - pulseAmount), baseColor.a * (1f + pulseAmount), t);
        img.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
    }
}
