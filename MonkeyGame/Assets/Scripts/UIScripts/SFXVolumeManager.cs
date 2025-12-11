using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeManager : MonoBehaviour
{
    [Header("Optional: assign only in scenes with a slider")]
    public Slider sfxSlider; // leave empty in scenes without a slider

    AudioSource[] sfxSources;
    float currentVolume = 1f;

    void Awake()
    {
        // Load saved SFX volume (default = 1)
        currentVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    void Start()
    {
        // Find SFX sources in THIS scene
        RefreshSFXSources();

        // Apply loaded volume to all SFX here
        ApplyVolume(currentVolume);

        // Hook slider if we have one in this scene
        if (sfxSlider != null)
        {
            sfxSlider.value = currentVolume;
            sfxSlider.onValueChanged.AddListener(SetVolumeFromSlider);
        }
    }

    void OnDestroy()
    {
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(SetVolumeFromSlider);
        }
    }

    void RefreshSFXSources()
    {
        GameObject[] sfxObjects = GameObject.FindGameObjectsWithTag("SFX");
        sfxSources = new AudioSource[sfxObjects.Length];

        for (int i = 0; i < sfxObjects.Length; i++)
        {
            sfxSources[i] = sfxObjects[i].GetComponent<AudioSource>();
        }
    }

    public void SetVolumeFromSlider(float volume)
    {
        currentVolume = volume;
        ApplyVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    void ApplyVolume(float volume)
    {
        if (sfxSources == null) return;

        foreach (AudioSource src in sfxSources)
        {
            if (src != null)
                src.volume = volume;
        }
    }
}
