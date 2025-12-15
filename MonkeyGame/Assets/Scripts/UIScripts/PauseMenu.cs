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

[Header("Input (UI map)")]
[Tooltip("Bind to UI/Cancel or UI/Pause so ESC/B/Start will close while open")]
public InputActionReference closeAction;

bool listening = false;

[SerializeField] GameObject pauseMenu;

    public void Pause()
    {
        if (panelRoot != null) panelRoot.SetActive(true);

        if (firstSelected && EventSystem.current)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
        StartListening();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        MusicManager.Instance.PauseMusic();
        if (SFXManager.instance != null) SFXManager.instance.PauseLoop();
    }

    public void Resume()
    {
        StopListening();

        if(panelRoot != null) panelRoot.SetActive(false);

        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        MusicManager.Instance.ResumeMusic();
        if (SFXManager.instance != null) SFXManager.instance.ResumeLoop();
    }

    public void Home()
    {
        if (SFXManager.instance != null) SFXManager.instance.StopLoop();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        MusicManager.Instance.ResumeMusic();
    }

    public void Restart()
    {
        if (SFXManager.instance != null) SFXManager.instance.StopLoop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        MusicManager.Instance.ResumeMusic();
    }

    public void OnResumeButton()
    {
        GameManager.Instance?.ResumeFromPause();
    }

    void StartListening()
    {
        if (listening) return;
        if (closeAction != null)
        {
            closeAction.action.performed += OnClosePerformed;
            closeAction.action.Enable();
            listening = true;
        }
    }

    void StopListening()
    {
        if (!listening) return;
        if (closeAction != null)
        {
            closeAction.action.performed -= OnClosePerformed;
            closeAction.action.Disable();
        }
        listening = false;
    }

    void OnDisable() => StopListening();

    private void OnClosePerformed(InputAction.CallbackContext _)
    {
        GameManager.Instance?.ResumeFromPause();
    }
}
