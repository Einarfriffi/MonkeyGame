using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIFader : MonoBehaviour
{
    public CanvasGroup uiGroup; 
    public float fadeDuration = 0.4f;

    [Header("Sound")]
    public AudioSource sfxSource;    // AudioSource for UI sounds
    public AudioClip clickSound;     // Optional: assign a click sound

    void Start()
    {
        uiGroup.alpha = 0f;
        StartCoroutine(FadeIn());
    }

    public void FadeToNextScene(string sceneName)
    {
        PlayClickSound();  // 🔊 play sound when transition starts
        StartCoroutine(FadeOutAndSwitch(sceneName));
    }

    public void PlayClickSound()
    {
        if (sfxSource == null) return;

        if (clickSound != null)
        {
            // play this specific clip
            sfxSource.PlayOneShot(clickSound);
        }
        else
        {
            // or just play whatever clip is on the AudioSource
            sfxSource.Play();
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            uiGroup.alpha = t / fadeDuration;
            yield return null;
        }
    }

    IEnumerator FadeOutAndSwitch(string sceneName)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            uiGroup.alpha = 1f - (t / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
