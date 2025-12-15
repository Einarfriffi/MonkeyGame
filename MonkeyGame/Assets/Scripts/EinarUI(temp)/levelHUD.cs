using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class levelHUD : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;
    
    [Header("Timer Panel")]
    public GameObject timerPanel;

    private GameObject pauseButton;
    private float elapsedTime;
    private bool timerRunning = false;
    private bool isTutorial = false;
    
    public float ElapsedTime => elapsedTime;
    public float StopAndGetTime() { timerRunning = false; return elapsedTime; }

    void Start()
    {
        FindPauseButton();
        CheckIfTutorial();
    }

    void CheckIfTutorial()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        isTutorial = sceneName.Contains("Tutorial") || sceneName.Contains("tutorial");
        
        if (isTutorial)
        {
            if (timerText != null)
                timerText.gameObject.SetActive(false);
            
            if (timerPanel != null)
                timerPanel.SetActive(false);
        }
    }

    void FindPauseButton()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.name.Contains("Pause"))
            {
                pauseButton = btn.gameObject;
                Debug.Log($"Found pause button: {pauseButton.name} at path: {GetGameObjectPath(pauseButton)}");
                break;
            }
        }

        if (pauseButton == null)
        {
            Debug.LogWarning("Could not find PauseButton in scene");
        }
    }

    string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return path;
    }

    void Update()
    {   
        if (!timerRunning || isTutorial) return;

        elapsedTime += Time.deltaTime;
        timerText.text = FormatTime(elapsedTime);
    }

    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}".PadLeft(8, '0');
    }

    public void SetLevelNumber(int level)
    {
        levelText.text = $"Level {level}";
    }

    public void SetLevelName(string name)
    {
        levelText.text = name;
    }

    public void StartTimer()
    {
        if (isTutorial) return;
        
        elapsedTime = 0f;
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void HidePauseButton()
    {
        if (pauseButton == null) FindPauseButton();
        
        Debug.Log($"HidePauseButton called. pauseButton is {(pauseButton != null ? "assigned" : "NULL")}");
        if (pauseButton != null)
        {
            Debug.Log($"Hiding pause button. Was active: {pauseButton.activeSelf}");
            pauseButton.SetActive(false);
        }
    }

    public void ShowPauseButton()
    {
        if (pauseButton == null) FindPauseButton();
        
        if (pauseButton != null)
            pauseButton.SetActive(true);
    }
}
