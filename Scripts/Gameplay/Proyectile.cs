using UnityEngine;
using System.Collections;

public class Proyectile : MonoBehaviour
{
    // Este script gestiona todos los proyectiles, tanto los de dano como los power ups

    [SerializeField]
    private float speed = 5;                 // Velocidad del proyectil
    [SerializeField]
    private float lifeTime = 2;              // Tiempo de vida antes de autodestruirse
    [SerializeField]
    private int healthMod = 0;               // Lo que modifica la vida objetivo
    [SerializeField]
    private string objetiveTag = null;       // Tag del objetivo
    [SerializeField]
    private AudioClip collisionAudio;        // Sonido al impactar
    [SerializeField] 
    private ParticleSystem deathParticle;    // Particulas al impactar

    private Coroutine lifeCoroutine;         // Referencia a la corrutina de vida para poder cancelarla si colisiona

    void Awake(){
        // Activa el temporizador de vida
        lifeCoroutine = StartCoroutine(ProyectileLife());
    }

    private void Update()
    {
        // Se mueve si el juego no esta en pausa
        if(!GameOrchestrator.instance.gamePaused)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private IEnumerator ProyectileLife()
    {
        // Si pasa el tiempo que se ha establecido se destruye (para que no se llene la escena)
        yield return GameOrchestrator.instance.PausableWait(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other){
        // Solo ejecutarlo si no se exige tag o si coincide con el del objetivo
        if (string.IsNullOrEmpty(objetiveTag) || other.gameObject.CompareTag(objetiveTag))
        {
            // Cancelar la corutina
            if (lifeCoroutine != null)
                StopCoroutine(lifeCoroutine);

            // Aplicar el cambio de vida al objetivo si tiene LifeSystem
            LifeSystem lifeObjetive = other.GetComponent<LifeSystem>();

            if (lifeObjetive != null)
            {
                lifeObjetive.ChangeHealth(healthMod);
            }

            // Sonido de impacto
            if(collisionAudio!=null){
                SoundManager.instance.PlayAudioClip(collisionAudio);
            }

            // Particulas de impacto (se separan para que sigan tras destruir el proyectil)
            if(deathParticle!=null){
                deathParticle.transform.parent=null;
                deathParticle.Play();
            }

            // Destruir el proyectil
            Destroy(gameObject);
        }
    }
}
