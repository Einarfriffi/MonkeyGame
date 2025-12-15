using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel & Focus")]
    [Tooltip("Root panel")]
    public GameObject panelRoot;

    [Tooltip("First button to select")]
    public Selectable firstSelected;

    [Header("Input Action")]
    [Tooltip("Pause/Unpause action (bind to ESC key)")]
    public InputActionReference pauseAction;

    private bool isPaused = false;
    private bool canPause = true;

    void Awake()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed += OnPausePressed;
            
            var levelIntro = Object.FindFirstObjectByType<LevelIntro>(FindObjectsInactive.Include);
            if (levelIntro == null)
            {
                pauseAction.action.Enable();
                canPause = true;
                Debug.Log("PauseMenu Awake: No LevelIntro found, pause ENABLED");
            }
            else
            {
                canPause = false;
                Debug.Log("PauseMenu Awake: LevelIntro found, pause DISABLED");
            }
        }
        else
        {
            Debug.LogError("PauseMenu: pauseAction is NULL! Assign it in the Inspector.");
        }
        
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    void OnDestroy()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPausePressed;
        }
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        Debug.Log($"PauseMenu: OnPausePressed called! canPause={canPause}, isPaused={isPaused}");
        
        if (!canPause) return;

        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void EnablePausing()
    {
        canPause = true;
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            Debug.Log($"PauseMenu: EnablePausing called. Action enabled={pauseAction.action.enabled}");
        }
    }

    public void DisablePausing()
    {
        canPause = false;
        if (pauseAction != null)
        {
            pauseAction.action.Disable();
            Debug.Log("PauseMenu: DisablePausing called");
        }
    }

    public void Pause()
    {
        if (isPaused || !canPause) return;
        isPaused = true;

        Debug.Log("PauseMenu: PAUSED");

        if (panelRoot != null) panelRoot.SetActive(true);

        if (firstSelected && EventSystem.current)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }

        Time.timeScale = 0;
        
        if (MusicManager.Instance != null) MusicManager.Instance.PauseMusic();
        if (SFXManager.instance != null) SFXManager.instance.PauseLoop();

        if (GameManager.Instance != null && GameManager.Instance.playerInput != null)
            GameManager.Instance.playerInput.DeactivateInput();
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        Debug.Log("PauseMenu: RESUMED");

        if (panelRoot != null) panelRoot.SetActive(false);

        Time.timeScale = 1;
        
        if (MusicManager.Instance != null) MusicManager.Instance.ResumeMusic();
        if (SFXManager.instance != null) SFXManager.instance.ResumeLoop();

        if (GameManager.Instance != null && GameManager.Instance.playerInput != null)
            GameManager.Instance.playerInput.ActivateInput();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnResumeButton()
    {
        Resume();
    }

    public void Home()
    {
        if (SFXManager.instance != null) SFXManager.instance.StopLoop();
        Time.timeScale = 1;
        if (MusicManager.Instance != null) MusicManager.Instance.ResumeMusic();
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        if (SFXManager.instance != null) SFXManager.instance.StopLoop();
        Time.timeScale = 1;
        if (MusicManager.Instance != null) MusicManager.Instance.ResumeMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
