using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [Header("Loads this scene when pressing Enter")]
    public string sceneNameOnEnter;

    [Header("Assign your UIFader here")]
    public UIFader fader;

    [Header("Disable auto-transition in these scenes")]
    public string[] scenesToDisableAutoTransition = { "Settings", "MainMenu" };

    [Header("Back Navigation (ESC key)")]
    public bool enableEscapeBack = false;
    public string escapeBackScene = "MainMenu";

    private bool autoTransitionEnabled = true;

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        foreach (string sceneName in scenesToDisableAutoTransition)
        {
            if (currentScene.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                autoTransitionEnabled = false;
                Debug.Log($"Auto-transition disabled in {currentScene}");
                break;
            }
        }
    }

    void Update()
    {
        if (enableEscapeBack && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            LoadSceneByName(escapeBackScene);
            return;
        }

        if (!autoTransitionEnabled) return;

        bool shouldTransition = false;

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            shouldTransition = true;
        }

        if (Mouse.current != null &&
            (Mouse.current.leftButton.wasPressedThisFrame ||
             Mouse.current.rightButton.wasPressedThisFrame ||
             Mouse.current.middleButton.wasPressedThisFrame))
        {
            shouldTransition = true;
        }

        if (shouldTransition && !string.IsNullOrEmpty(sceneNameOnEnter))
        {
            fader.FadeToNextScene(sceneNameOnEnter);
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        fader.FadeToNextScene(sceneName);
    }
}
