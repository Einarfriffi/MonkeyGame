using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    [Header("First Button")]
    public Selectable firstSelected;

    void OnEnable()
    {
        if (firstSelected != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }

    public void NextLevel()
    {
        PrepareSceneTransition();
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No next level, going to main menu");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Restart()
    {
        PrepareSceneTransition();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        PrepareSceneTransition();
        SceneManager.LoadScene("MainMenu");
    }

    void PrepareSceneTransition()
    {
        StopAllCoroutines();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StopAllCoroutines();
        }
        
        if (SFXManager.instance != null) SFXManager.instance.StopLoop();
        Time.timeScale = 1;
        if (MusicManager.Instance != null) MusicManager.Instance.ResumeMusic();
    }
}
