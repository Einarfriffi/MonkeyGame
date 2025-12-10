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
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (!string.IsNullOrEmpty(sceneNameOnEnter))
            {
                // FADE instead of instantly loading
                fader.FadeToNextScene(sceneNameOnEnter);
            }
            else
            {
                Debug.LogWarning("SceneManagerScript: sceneNameOnEnter is empty!");
            }
        }
    }

    // For UI buttons (optional)
    public void LoadSceneByName(string sceneName)
    {
        fader.FadeToNextScene(sceneName);
    }
}
