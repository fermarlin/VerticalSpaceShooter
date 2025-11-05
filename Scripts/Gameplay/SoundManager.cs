using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Este script es un gestor de sonidos global que puedo llamar desde donde sea, tiene un pool de audiosources que voy activando cuando hace falta

    private float low = 0.75f;                  // Limite inferior de variacion de pitch aleatorio
    private float high = 1.25f;                 // Limite superior de variacion de pitch aleatorio
    private AudioSource[] audioSources;         // Pool de AudioSources en este GameObject
    public static SoundManager instance;        // Acceso global

    private void Awake(){
        // Inicializo para que pueda acceder donde sea y pillo los AudioSources del GameObject
        if (instance == null)
        {
            instance = this;
        }

        audioSources = GetComponents<AudioSource>();
    }

    public void PlayAudioClip(AudioClip audioC){
        // Reproduce un clip con un pitch aleatorio dentro de los limites, esto lo hago para que no de fatiga el mismo sonido todo el rato
        int i = CheckAudioSourceFree();
        audioSources[i].pitch = Random.Range(low, high);
        audioSources[i].PlayOneShot(audioC);
    }

    public void PlayAudioClipDefaultPitch(AudioClip audioC){
        // Reproduce un clip con pitch por defecto (1.0)
        int i = CheckAudioSourceFree();
        audioSources[i].pitch = 1f;
        audioSources[i].PlayOneShot(audioC);
    }

    public int CheckAudioSourceFree(){
        // Devuelve el primer AudioSource que no este reproduciendo.
        // Si todos estan ocupados, devuelve el 0 como fallback.
        for(int i=0; i<audioSources.Length; i++){
            if(!audioSources[i].isPlaying){
                return i;
            }
        }
        return 0;
    }

    public void AudioCancel(){
        // Detiene todos los audios
        for(int i=0; i<audioSources.Length; i++){
            audioSources[i].Stop();
        }
    } 
}
