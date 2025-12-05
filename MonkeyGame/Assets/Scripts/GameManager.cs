using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int currentLevel = 1;
    public GameObject deathPanel;
    public GameObject HUDCanvas;
    private GameObject currentHUD;
    private levelHUD levelHUD;




    void Awake()
    {
        // Singleton
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene lodade: " + scene.name);
        if (scene.name.StartsWith("Level"))
        {
            GameObject deathUIfound = GameObject.FindWithTag("DeathUI");
            if (deathUIfound != null)
            {
                var panelTransform = deathUIfound.GetComponentInChildren<Transform>(true)
                                                .Cast<Transform>()
                                                .FirstOrDefault(t => t.name == "DeathPanel");
                
                if (panelTransform != null)
                {
                    deathPanel = panelTransform.gameObject;
                    deathPanel.SetActive(false);
                }
            }

            if (HUDCanvas != null && currentHUD == null)
            {
                currentHUD = Instantiate(HUDCanvas);
                levelHUD = currentHUD.GetComponent<levelHUD>();
                levelHUD.SetLevelNumber(currentLevel);
            }
        }
        else
        {
            if (currentHUD != null)
            {
                Destroy(currentHUD);
                currentHUD = null;
                levelHUD = null;
            }
        }
    }

    private IEnumerator DeathSlowdownRoutine()
    {
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

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
    }

    public void ShowDeathScreen()
    {
        StartCoroutine(DeathSlowdownRoutine());
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelWon()
    {
        SceneManager.LoadScene("DevSplash");
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
}
