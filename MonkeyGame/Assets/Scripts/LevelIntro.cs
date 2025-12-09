using System.Collections;
using UnityEngine;
using TMPro;

public class LevelIntro : MonoBehaviour
{
    // text to display for countdown
    [Header("UI Elements")]
    public TextMeshProUGUI countdownText;

    // length of countdown
    [Header("Settings")]
    public float countdownTime = 3f;

    // enables all scene objects when countdown is done
    [Header("Gameplay Elements")]
    public GameObject[] objectsToEnableAfterCountdown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // make sure compenents are assigned
        if (countdownText == null)
        {
            countdownText = GetComponentInChildren<TextMeshProUGUI>();
        }
        // Pause game while countdown going
        GameManager.Instance.PauseGame();
        
        // Call level start countdown
        StartCoroutine(CountdownAndStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Coutdown for delayed level start
    IEnumerator CountdownAndStart()
    {
        float currentTime = countdownTime;

        while (currentTime > 0)
        {
            countdownText.text = Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSecondsRealtime(1f);
            currentTime --;
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);

        // hide countdown ui
        countdownText.gameObject.SetActive(false);

        // activate all scene game objects (need to assign each in LeveLoader)
        foreach (GameObject obj in objectsToEnableAfterCountdown)
        {
            obj.SetActive(true);
        }

        // Start Level
        GameManager.Instance.StartGame();

        // Start Stopwatch timer
        Object.FindFirstObjectByType<levelHUD>()?.StartTimer();
    }
}
