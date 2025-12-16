using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Scene Navigation")]
    public string backSceneName = "MainMenu";
    
    [Header("UI Fader (optional)")]
    public UIFader fader;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GoBack();
        }
        
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonEast.wasPressedThisFrame || 
                Gamepad.current.startButton.wasPressedThisFrame)
            {
                GoBack();
            }
        }
    }

    public void ChangeGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log("Switched to quality: " + QualitySettings.names[index]);
    }

    public void GoBack()
    {
        if (fader != null)
        {
            fader.FadeToNextScene(backSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(backSceneName);
        }
    }
}
