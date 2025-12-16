using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.AI;
using NUnit.Framework;

public class GameManager : MonoBehaviour
{
    // Game manager vars
    public static GameManager Instance { get; private set; }
    public float lastCompletedTime { get; private set; }
    public PlayerInput playerInput;
    public int currentLevel = 1;
    public GameObject deathPanel;
    public GameObject winScreen;
    public GameObject HUDCanvas;
    public GameObject parent;
    private GameObject currentHUD;
    private levelHUD levelHUD;
    private bool startInUIMode = true;

    void Awake()
    {
        // Iniciate Singleton Instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (startInUIMode && playerInput != null)
            playerInput.DeactivateInput();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Filling game manager on each scene load depending on what is needed for each scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        Debug.Log("Scene lodade: " + scene.name);
        startInUIMode = true;
        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }

        // fetch Level scenes GameManager components
        if (scene.name.StartsWith("Level") && scene.name != "LevelManager")
        {
            startInUIMode = false;
            // fetch win screen
            GameObject winScreenObj = GameObject.FindWithTag("WinScreen");
            if (winScreenObj != null)
            {
                var panelTransform = winScreenObj.GetComponentInChildren<Transform>(true)
                                                .Cast<Transform>()
                                                .FirstOrDefault(t => t.name == "Menu");
                if (panelTransform != null)
                {
                    winScreen = panelTransform.gameObject;
                    winScreen.SetActive(false);
                }
                var timerParent = winScreen.GetComponentInChildren<Transform>(true)
                                            .Cast<Transform>()
                                            .FirstOrDefault(t => t.name == "Panel");
                if (timerParent != null)
                {
                    parent = timerParent.gameObject;
                }
                
            }
            // fetch player object
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerInput = playerObj.GetComponent<PlayerInput>();
            }

            if (!startInUIMode && playerInput != null) playerInput.ActivateInput();
    
            // fetch deathUI for game manager
            GameObject deathUIfound = GameObject.FindWithTag("DeathUI");
            if (deathUIfound != null)
            {
                var panelTransform = deathUIfound.GetComponentInChildren<Transform>(true)
                                                .Cast<Transform>()
                                                .FirstOrDefault(t => t.name == "Menu");

                if (panelTransform != null)
                {
                    deathPanel = panelTransform.gameObject;
                    deathPanel.SetActive(false);
                }
            }

            // Fetch HUD for game manager
            if (HUDCanvas != null && currentHUD == null)
            {
                currentHUD = Instantiate(HUDCanvas);
                levelHUD = currentHUD.GetComponent<levelHUD>();
                levelHUD.SetLevelNumber(currentLevel);
            }
            // TODO: Fetch Pause Button
        }
        else
        {
            // Reset level HUD for next level
            if (currentHUD != null)
            {
                Destroy(currentHUD);
                currentHUD = null;
                levelHUD = null;
            }
        }

        if (scene.name.StartsWith("Tutorial"))
        {
            startInUIMode = false;

            // fetch player object
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerInput = playerObj.GetComponent<PlayerInput>();
            }

            if (!startInUIMode && playerInput != null) playerInput.ActivateInput();

            // fetch deathUI for game manager
            GameObject deathUIfound = GameObject.FindWithTag("DeathUI");
            if (deathUIfound != null)
            {
                var panelTransform = deathUIfound.GetComponentInChildren<Transform>(true)
                                                .Cast<Transform>()
                                                .FirstOrDefault(t => t.name == "Menu");

                if (panelTransform != null)
                {
                    deathPanel = panelTransform.gameObject;
                    deathPanel.SetActive(false);
                }
            }
        }
    }

    // Slow Down for death
    // eventually pause
    // show death ui
    private IEnumerator DeathSlowdownRoutine()
    {
        if (playerInput != null)
            playerInput.enabled = false;
        
        var hud = FindFirstObjectByType<levelHUD>(FindObjectsInactive.Include);
        if (hud != null)
        {
            hud.StopTimer();
            hud.HidePauseButton();
        }

        var pauseMenu = FindFirstObjectByType<PauseMenu>(FindObjectsInactive.Include);
        if (pauseMenu != null)
        {
            pauseMenu.DisablePausing();
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
        
        float duration = 1.5f;
        float elapsed = 0f;
        float startScale = Time.timeScale;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, 0f, elapsed / duration);
            yield return null;
        }

        Time.timeScale = 0f;

    }

    // call death routine
    // call this when player dies
    public void ShowDeathScreen()
    {
        StartCoroutine(DeathSlowdownRoutine());
    }

    // Level Reset, reloads current level scene
    public void RestartLevel()
    {
        playerInput.enabled = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Call this for Winning screen
    public void LevelWon()
    {
        // TODO: add actual logic
        Time.timeScale = 0f;

        var hud = FindFirstObjectByType<levelHUD>(FindObjectsInactive.Include);
        float runSeconds = 0f;
        if (hud != null)
        {
            runSeconds = hud.StopAndGetTime();
            hud.HidePauseButton();
        }
        
        lastCompletedTime = runSeconds;

        var pauseMenu = FindFirstObjectByType<PauseMenu>(FindObjectsInactive.Include);
        if (pauseMenu != null)
        {
            pauseMenu.DisablePausing();
        }

        
        string formattedRun = levelHUD.FormatTime(runSeconds);

        string levelID = SceneManager.GetActiveScene().name;

        // record into leaderboard
        LeaderBoard.SubmitTime(levelID, runSeconds);
        
        string key = $"BestTime_{levelID}";
        float prevBest = PlayerPrefs.GetFloat(key, float.PositiveInfinity);

        bool improved = runSeconds > 0f && runSeconds < prevBest;
        if(improved)
        {
            PlayerPrefs.SetFloat(key, runSeconds);
            PlayerPrefs.Save();
        }
        float bestNow = improved ? runSeconds : prevBest;
        string formattedBestNow = levelHUD.FormatTime(bestNow);


        // set win screen active
        if (winScreen != null)
        {
            var runText = parent.transform.Find("Your Time") ?.GetComponent<TextMeshProUGUI>();
            if (runText != null) Debug.Log("found runtxt");
            var runBest = parent.transform.Find("Best Time") ?.GetComponent<TextMeshProUGUI>();
            if (runBest != null) Debug.Log("found runbest");

            if (runText) runText.text = $"This run: {formattedRun}";
            if (runBest) runBest.text = $"Best: { (float.IsPositiveInfinity(bestNow) ? "00:00:00" : formattedBestNow) }";
            winScreen.SetActive(true); 
        }
        // save time and send to db
    }

    public void TutorialWon()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Return game time to normal
    public void StartGame()
    {
        Time.timeScale = 1f;
    }
}
