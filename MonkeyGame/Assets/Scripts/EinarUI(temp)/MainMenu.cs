using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LEVEL_1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
