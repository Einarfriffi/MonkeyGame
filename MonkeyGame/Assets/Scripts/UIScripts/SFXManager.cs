using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private AudioSource loopingSource;

    private float masterVolume = 1f;
    public float MasterVolume => masterVolume;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // load saved volume
        masterVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        loopingSource = gameObject.AddComponent<AudioSource>();
        loopingSource.loop = true;
        loopingSource.playOnAwake = false;
        loopingSource.spatialBlend = 0f; // 2D by default
        loopingSource.volume = masterVolume;
    }

    // called by your slider script
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("SFXVolume", masterVolume);

        // update current loop immediately
        if (loopingSource != null)
            loopingSource.volume = masterVolume * (loopingSource.clip != null ? 1f : 1f);
    }

    public void PlaySoundEffect(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if (audioClip == null || soundFXObject == null) return;

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        // IMPORTANT: multiply by master volume
        audioSource.volume = volume * masterVolume;

        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
    }

    public void PlayLoop(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // already playing this clip
        if (loopingSource.clip == clip && loopingSource.isPlaying)
            return;

        loopingSource.clip = clip;

        // IMPORTANT: multiply by master volume
        loopingSource.volume = volume * masterVolume;

        loopingSource.Play();
    }

    public void StopLoop()
    {
        if (loopingSource != null && loopingSource.isPlaying)
            loopingSource.Stop();
    }
}
