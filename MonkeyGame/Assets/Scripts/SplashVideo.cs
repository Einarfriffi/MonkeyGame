using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SplashVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip customClip;
    public GameObject fadeCanvasPrefab;
    private Image fadeImage;
    public string nextScene = "LevelOne";
    public float fadeDuration = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject fadeCanvas = Instantiate(fadeCanvasPrefab);
        fadeImage = fadeCanvas.GetComponentInChildren<Image>();

        if (customClip != null)
        {
            videoPlayer.clip = customClip;
        }
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.anyKey.wasPressedThisFrame)
        {
            videoPlayer.Stop();
            StartCoroutine(FadeToBlack());
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(nextScene);
    }
}
