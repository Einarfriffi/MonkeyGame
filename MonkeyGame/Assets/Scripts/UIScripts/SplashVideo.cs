using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor.Search;

public class SplashVideo : MonoBehaviour
{
    // var for video player
    public VideoPlayer videoPlayer;
    // splash video path
    public string webglVideoUrl = "https://einarfriffi.github.io/MonkeyGame/Video/Logo_Animation_With_Electricity_Surge.mp4";
    public GameObject fadeCanvasPrefab;
    // image for transition fade
    private Image fadeImage;
    public float fadeDuration = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // fetch game object and fade image
        GameObject fadeCanvas = Instantiate(fadeCanvasPrefab);
        fadeImage = fadeCanvas.GetComponentInChildren<Image>();

        // set source and URL
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = webglVideoUrl;

        // Prep and play video
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => vp.Play();

        // call for transition fade when video ends
        videoPlayer.loopPointReached += OnVideoEnd;

    }

    // Update is called once per frame
    void Update()
    {
        // Any Key can skip video
        if(Keyboard.current.anyKey.wasPressedThisFrame)
        {
            videoPlayer.Stop();
            StartCoroutine(FadeToBlack());
        }
    }

    // Start transition fade
    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(FadeToBlack());
    }

    // Image fade logic
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

        SceneManager.LoadScene("StartScreen");
    }
}
