using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Transform leaderboardContainer;
    public GameObject leaderboardEntryPrefab;
    
    [Header("Settings")]
    public int minimumEntriesToShow = 10;
    public int maximumEntriesToShow = 50;
    
    [Header("Placeholder Settings")]
    public string emptyPlayerName = "Player Name";
    public string emptyTimeFormat = "--:--:---";
    
    void OnEnable()
    {
        RefreshLeaderboard();
    }
    
    public void RefreshLeaderboard()
    {
        if (LootLockerManager.Instance == null) return;
        
        LootLockerManager.Instance.GetLeaderboard((entries) =>
        {
            DisplayLeaderboard(entries);
        });
    }
    
    private void DisplayLeaderboard(List<LeaderboardEntry> entries)
    {
        ClearLeaderboard();
        
        int actualEntries = Mathf.Min(entries.Count, maximumEntriesToShow);
        int totalEntriesToDisplay = Mathf.Max(minimumEntriesToShow, actualEntries);
        
        for (int i = 0; i < totalEntriesToDisplay; i++)
        {
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            
            TMP_Text[] texts = entryObj.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 3)
            {
                int rank = i + 1;
                
                if (i < entries.Count)
                {
                    texts[0].text = $"#{entries[i].rank}";
                    texts[1].text = entries[i].playerName;
                    texts[2].text = FormatTime(entries[i].time);
                }
                else
                {
                    texts[0].text = $"#{rank}";
                    texts[1].text = emptyPlayerName;
                    texts[2].text = emptyTimeFormat;
                    
                    SetPlaceholderStyle(texts);
                }
            }
        }
    }
    
    private void SetPlaceholderStyle(TMP_Text[] texts)
    {
        Color placeholderColor = new Color(1f, 1f, 1f, 0.3f);
        
        foreach (TMP_Text text in texts)
        {
            if (text != null)
            {
                text.color = placeholderColor;
            }
        }
    }
    
    private void ClearLeaderboard()
    {
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        float secs = seconds % 60;
        return $"{minutes:00}:{secs:00.000}";
    }
}
