using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float low = 0.75f;
    private float high = 1.25f;
    private AudioSource[] audioSources;
    public static SoundManager instance;

    private void Awake(){

        if (instance == null)
        {
            instance = this;
        }

        audioSources = GetComponents<AudioSource>();
    }

    public void PlayAudioClip(AudioClip audioC){
        int i = CheckAudioSourceFree();
        audioSources[i].pitch = Random.Range(low, high);
        audioSources[i].PlayOneShot(audioC);
    }

    public void PlayAudioClipDefaultPitch(AudioClip audioC){
        int i = CheckAudioSourceFree();
        audioSources[i].pitch = 1f;
        audioSources[i].PlayOneShot(audioC);
    }


    public int CheckAudioSourceFree(){
        for(int i=0; i<audioSources.Length; i++){
            if(!audioSources[i].isPlaying){
                return i;
            }
        }
        return 0;
    }

    public void AudioCancel(){
        for(int i=0; i<audioSources.Length; i++){
            audioSources[i].Stop();
        }
    } 
}
