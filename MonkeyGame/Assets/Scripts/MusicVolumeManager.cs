using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeManager : MonoBehaviour
{
    public Slider musicSlider; // Assign your slider in inspector

    AudioSource[] musicSources;

    void Start()
    {
        // Find all music sources tagged "Music"
        GameObject[] musicObjects = GameObject.FindGameObjectsWithTag("Music");
        musicSources = new AudioSource[musicObjects.Length];

        for (int i = 0; i < musicObjects.Length; i++)
        {
            musicSources[i] = musicObjects[i].GetComponent<AudioSource>();
        }

        // Load saved volume (or default to 1)
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicSlider.value = savedVolume;
        ApplyVolume(savedVolume);

        // Listen to slider value change
        musicSlider.onValueChanged.AddListener(ApplyVolume);
    }

    void ApplyVolume(float volume)
    {
        foreach (AudioSource src in musicSources)
        {
            if (src != null)
                src.volume = volume;
        }

        // Save for future sessions
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
