using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIFader : MonoBehaviour
{
    public CanvasGroup uiGroup; 
    public float fadeDuration = 0.4f;

    [Header("Sound")]
    public AudioSource sfxSource; 
    public AudioClip clickSound; 
    [SerializeField, Range(0f, 1f)]
    private float clickVolume = 0.04f;

    void Start()
    {
        uiGroup.alpha = 0f;
        StartCoroutine(FadeIn());
    }

    public void FadeToNextScene(string sceneName)
    {
        PlayClickSound();  
        StartCoroutine(FadeOutAndSwitch(sceneName));
    }

    public void PlayClickSound()
    {
        if (sfxSource == null || clickSound == null) return;

        float master = (SFXManager.instance != null)
            ? SFXManager.instance.MasterVolume
            : 1f;

        sfxSource.PlayOneShot(clickSound, clickVolume * master);
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
