using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private AudioSource loopingSource;

    private void Awake()
    {
        if (instance == null)
        instance = this;

        loopingSource = gameObject.AddComponent<AudioSource>();
        loopingSource.loop = true;
        loopingSource.playOnAwake = false;
    }


    public void PlaySoundEffect(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of audioclip
        float clipLength = audioClip.length;

        //destroy gameObject after length of audioclip
        Destroy(audioSource.gameObject, clipLength);


        //AudioSource.PlayClipAtPoint(audioClip, spawnTransform.position);
    }

    public void PlayLoop(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        if (loopingSource.clip == clip && loopingSource.isPlaying)
            return;

        loopingSource.clip = clip;
        loopingSource.volume = volume;
        loopingSource.Play();
    }

    public void StopLoop()
    {
        if (loopingSource.isPlaying)
            loopingSource.Stop();
    }
}