using System.Collections;
using UnityEngine;
using TMPro;

public class LevelIntro : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI countdownText;

    [Header("Settings")]
    public float countdownTime = 3f;

    [Header("Gameplay Elements")]
    public GameObject[] objectsToEnableAfterCountdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (countdownText == null)
        {
            countdownText = GetComponentInChildren<TextMeshProUGUI>();
        }
        GameManager.Instance.PauseGame();

        StartCoroutine(CountdownAndStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

        countdownText.gameObject.SetActive(false);

        foreach (GameObject obj in objectsToEnableAfterCountdown)
        {
            obj.SetActive(true);
        }

        GameManager.Instance.StartGame();
        Object.FindFirstObjectByType<levelHUD>()?.StartTimer();
    }
}
