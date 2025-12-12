using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    
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


        AudioSource.PlayClipAtPoint(audioClip, spawnTransform.position);
    }
}