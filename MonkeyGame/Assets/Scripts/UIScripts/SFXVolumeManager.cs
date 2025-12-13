using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeManager : MonoBehaviour
{
    [Header("Assign in Settings scene only")]
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        // Load saved value (default = 1)
        float saved = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Apply immediately to SFXManager if it exists
        if (SFXManager.instance != null)
            SFXManager.instance.SetMasterVolume(saved);

        // If slider exists, set its UI value too
        if (sfxSlider != null)
            sfxSlider.value = saved;
    }

    private void OnEnable()
    {
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable()
    {
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);

        if (SFXManager.instance != null)
            SFXManager.instance.SetMasterVolume(value);
    }
}
