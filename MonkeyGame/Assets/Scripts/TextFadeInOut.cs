using UnityEngine;

public class TextFadeInOut : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;
    public float minAlpha = 0f; // Lowest transparency
    public float maxAlpha = 1f; // Highest transparency

    private bool fadingOut = false;

    void Update()
    {
        float target = fadingOut ? minAlpha : maxAlpha;

        // Move alpha toward target
        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha,
            target,
            Time.deltaTime / fadeDuration
        );

        // Fade back when reaching limits
        if (Mathf.Approximately(canvasGroup.alpha, target))
        {
            fadingOut = !fadingOut;
        }
    }
}