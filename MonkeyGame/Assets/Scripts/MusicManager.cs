using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public string[] stopMusicInScenes; // Scenes where music should stop

    AudioSource audioSource;

    void Awake()
    {
        // Keep only ONE MusicManager
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        // Listen to scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If scene is in the stop list → stop music
        foreach (string sceneName in stopMusicInScenes)
        {
            if (scene.name == sceneName)
            {
                audioSource.Stop();
                return;
            }
        }

        // Otherwise → start playing if not already
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
