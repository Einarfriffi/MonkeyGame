using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;   // << new input system

public class SceneManagerScript : MonoBehaviour
{
    [Header("Loads this scene when pressing Enter")]
    public string sceneNameOnEnter;

    void Update()
    {
        // Keyboard.current can be null in some cases, so we check it first
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (!string.IsNullOrEmpty(sceneNameOnEnter))
            {
                SceneManager.LoadScene(sceneNameOnEnter);
            }
            else
            {
                Debug.LogWarning("SceneManagerScript: sceneNameOnEnter is empty!");
            }
        }
    }

    // Still works for UI buttons etc.
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
