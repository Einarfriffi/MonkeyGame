using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Game manager vars
    public static GameManager Instance { get; private set; }
    public PlayerInput playerInput;
    public int currentLevel = 1;
    public GameObject deathPanel;
    public GameObject HUDCanvas;
    private GameObject currentHUD;
    private levelHUD levelHUD;




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

        // fetch Level scenes GameManager components
        if (scene.name.StartsWith("Level") && scene.name != "LevelManager")
        {

            // fetch win screen
            GameObject winScreen = GameObject.FindWithTag("Win");
            if (winScreen != null)
            {
                // set active false stuff
            }
            // fetch player object
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerInput = playerObj.GetComponent<PlayerInput>();

                if (playerInput == null)
                    Debug.Log("Player found but not input");
                else
                    Debug.Log("player input assigned");
            }
            else
            {
                Debug.Log("No player found in scene");
            }

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
        // set win screen active
        // save time and send to db

        SceneManager.LoadScene("DevSplash");
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

    // Freeze game time
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
}
