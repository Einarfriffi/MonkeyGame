using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public Button firstSelected;
    
    [Header("UI Panels")]
    public GameObject resultsPanel;
    public GameObject nameInputDialog;
    
    [Header("Leaderboard Settings")]
    public int topScoreThreshold = 50;
    
    void OnEnable()
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.StopLoop();
        }
        
        CheckAndSubmitScore();
        
        if (firstSelected != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }
    
    private void CheckAndSubmitScore()
    {
        if (LootLockerManager.Instance == null) return;
        
        float currentTime = GetCurrentLevelTime();
        if (currentTime <= 0) return;
        
        LootLockerManager.Instance.GetLeaderboard((entries) =>
        {
            if (LootLockerManager.Instance.IsTopScore(currentTime, entries, topScoreThreshold))
            {
                ShowNameInputDialog(currentTime);
            }
            else
            {
                ShowResultsPanel();
                LootLockerManager.Instance.SubmitScore(currentTime);
            }
        });
    }
    
    private void ShowNameInputDialog(float scoreTime)
    {
        if (resultsPanel != null)
            resultsPanel.SetActive(false);
        
        if (nameInputDialog != null)
        {
            nameInputDialog.SetActive(true);
            
            NameInputDialog dialog = nameInputDialog.GetComponent<NameInputDialog>();
            if (dialog != null)
            {
                dialog.Show((playerName) =>
                {
                    OnNameSubmitted(scoreTime);
                });
            }
        }
    }
    
    private void OnNameSubmitted(float scoreTime)
    {
        if (nameInputDialog != null)
            nameInputDialog.SetActive(false);
        
        if (resultsPanel != null)
            resultsPanel.SetActive(true);
        
        LootLockerManager.Instance.SubmitScore(scoreTime);
        
        if (firstSelected != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }
    
    private void ShowResultsPanel()
    {
        if (nameInputDialog != null)
            nameInputDialog.SetActive(false);
        
        if (resultsPanel != null)
            resultsPanel.SetActive(true);
    }
    
    private float GetCurrentLevelTime()
    {
        if (GameManager.Instance != null)
        {
            return GameManager.Instance.lastCompletedTime;
        }
        return 0f;
    }
    
    public void NextLevel()
    {
        PrepareSceneTransition();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Restart()
    {
        PrepareSceneTransition();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void Home()
    {
        PrepareSceneTransition();
        SceneManager.LoadScene("MainMenu");
    }
    
    private void PrepareSceneTransition()
    {
        StopAllCoroutines();
        if (GameManager.Instance != null) GameManager.Instance.StopAllCoroutines();
        if (SFXManager.instance != null) SFXManager.instance.StopLoop();
        Time.timeScale = 1;
        if (MusicManager.Instance != null) MusicManager.Instance.ResumeMusic();
    }
}
