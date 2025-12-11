using TMPro;
using UnityEngine;

public class levelHUD : MonoBehaviour
{
    // Var for text components
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;

    // track time elapsed
    private float elapsedTime;
    private bool timerRunning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        // if Level not started do nothing
        if (!timerRunning) return;

        // else update stopwatch for level duration
        elapsedTime += Time.deltaTime;
        timerText.text = FormatTime(elapsedTime);
    }

    // change stopwatch to string for UI display
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    // Set Level text for component
    public void SetLevelNumber(int level)
    {
        levelText.text = $"Level {level}";
    }

    // Call when level starts
    public void StartTimer()
    {
        elapsedTime = 0f;
        timerRunning = true;
    }

    // should probably add ResumeTimer() here
    
    // Call to freeze timer
    // level done or paused called
    public void StopTimer()
    {
        timerRunning = false;
    }
}
