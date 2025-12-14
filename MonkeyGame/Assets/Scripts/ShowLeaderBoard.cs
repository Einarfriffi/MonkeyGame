using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShowLeaderBoard : MonoBehaviour
{
    [Header("Assign the 5 text fields (Top 1 at index 0)")]
    public TextMeshProUGUI[] lines = new TextMeshProUGUI[5];

    [Header("Which level’s board to show")]
    public string levelId = "Level1"; // or set at runtime when user selects a level

    // Optional: helper to format, reusing your existing formatter if you want
    private string FormatTime(float t)
    {
        // If you prefer to reuse your HUD formatter:
        // return levelHUD.FormatTime(t);
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        int milliseconds = Mathf.FloorToInt((t * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public void ShowBoard(string newLevelId = null)
    {
        if (!string.IsNullOrEmpty(newLevelId))
            levelId = newLevelId;

        List<float> times = LeaderBoard.GetTopTimes(levelId);

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == null) continue;

            if (i < times.Count)
                lines[i].text = $"{i+1}.  {FormatTime(times[i])}";
            else
                lines[i].text = $"{i+1}.  --:--:--";
        }
    }

    void OnEnable()
    {
        // Populate when menu opens
        ShowBoard(levelId);
    }
}
