using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class LootLockerManager : MonoBehaviour
{
    public static LootLockerManager Instance { get; private set; }
    
    [Header("Leaderboard Settings")]
    public string leaderboardKey = "speedrun_times";
    public int maxLeaderboardEntries = 100;
    
    private bool isInitialized = false;
    private string playerName = "";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        StartCoroutine(LoginRoutine());
    }
    
    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("LootLocker: Session started successfully!");
                isInitialized = true;
                
                playerName = PlayerPrefs.GetString("PlayerName", "");
                if (!string.IsNullOrEmpty(playerName))
                {
                    SetPlayerName(playerName);
                }
            }
            else
            {
                Debug.LogError($"LootLocker: Failed to start session: {response.errorData.message}");
            }
            done = true;
        });
        
        yield return new WaitUntil(() => done);
    }
    
    public void SetPlayerName(string newName, Action<bool> callback = null)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("LootLocker not initialized yet!");
            callback?.Invoke(false);
            return;
        }
        
        playerName = newName;
        PlayerPrefs.SetString("PlayerName", newName);
        PlayerPrefs.Save();
        
        LootLockerSDKManager.SetPlayerName(newName, (response) =>
        {
            if (response.success)
            {
                Debug.Log($"Player name set to: {newName}");
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogError($"Failed to set player name: {response.errorData.message}");
                callback?.Invoke(false);
            }
        });
    }
    
    public void SubmitScore(float timeInSeconds, Action<bool> callback = null)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("LootLocker not initialized yet!");
            callback?.Invoke(false);
            return;
        }
        
        int scoreInMilliseconds = Mathf.RoundToInt(timeInSeconds * 1000);
        
        LootLockerSDKManager.SubmitScore("", scoreInMilliseconds, leaderboardKey, (response) =>
        {
            if (response.success)
            {
                Debug.Log($"Score submitted: {timeInSeconds:F3}s");
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogError($"Failed to submit score: {response.errorData.message}");
                callback?.Invoke(false);
            }
        });
    }
    
    public void GetLeaderboard(Action<List<LeaderboardEntry>> callback)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("LootLocker not initialized yet!");
            callback?.Invoke(new List<LeaderboardEntry>());
            return;
        }
        
        LootLockerSDKManager.GetScoreList(leaderboardKey, maxLeaderboardEntries, 0, (response) =>
        {
        if (response.success)
        {
            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
            
            if (response.items != null)
            {
                foreach (var item in response.items)
                {
                    if (item == null) continue;
                    
                    string name = "Anonymous";
                    if (item.player != null && !string.IsNullOrEmpty(item.player.name))
                    {
                        name = item.player.name;
                    }
                    
                    float time = item.score / 1000f;
                    int rank = item.rank;
                    
                    entries.Add(new LeaderboardEntry(name, time, rank));
                }
            }
            
            callback?.Invoke(entries);
        }

        });
    }
    
    public bool IsTopScore(float timeInSeconds, List<LeaderboardEntry> leaderboard, int topCount = 5)
    {
        if (leaderboard.Count < topCount) return true;
        
        for (int i = 0; i < Mathf.Min(topCount, leaderboard.Count); i++)
        {
            if (timeInSeconds < leaderboard[i].time)
            {
                return true;
            }
        }
        
        return false;
    }
}

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;
    public int rank;
    
    public LeaderboardEntry(string name, float t, int r)
    {
        playerName = name;
        time = t;
        rank = r;
    }
}
