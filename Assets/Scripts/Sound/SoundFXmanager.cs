using System.Linq.Expressions;
using UnityEngine;

public class SoundFXmanager : MonoBehaviour
{
   public static SoundFXmanager instance;

   [SerializeField] private AudioSource soundFXObject;

   private void Awake()
   {
        if (instance == null)
        {
            instance = this;
        }
   }


    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip =audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //audioclip length
        float clipLength = audioSource.clip.length;

        //destroy clip
        Destroy(audioSource.gameObject,clipLength);
    }
}