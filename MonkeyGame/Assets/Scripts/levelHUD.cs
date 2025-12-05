using TMPro;
using UnityEngine;

public class levelHUD : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;

    private float elapsedTime;
    private bool timerRunning = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!timerRunning) return;

        elapsedTime += Time.deltaTime;
        timerText.text = FormatTime(elapsedTime);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public void SetLevelNumber(int level)
    {
        levelText.text = $"Level {level}";
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }
}
