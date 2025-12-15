using UnityEngine;
using UnityEngine.InputSystem;   // new input system

public class SceneManagerScript : MonoBehaviour
{
    [Header("Loads this scene when pressing Enter")]
    public string sceneNameOnEnter;

    [Header("Assign your UIFader here")]
    public UIFader fader;

    void Update()
    {
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

    // For UI buttons (optional)
    public void LoadSceneByName(string sceneName)
    {
        fader.FadeToNextScene(sceneName);
    }
}
