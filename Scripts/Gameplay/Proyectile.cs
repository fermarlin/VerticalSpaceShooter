using UnityEngine;
using System.Collections;

public class Proyectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;
    [SerializeField]
    private float lifeTime = 2;
    [SerializeField]
    private int healthMod = 0; 
    [SerializeField]
    private string objetiveTag = null; 
    [SerializeField]
    private AudioClip pickUpAudio; 
    [SerializeField] 
    private ParticleSystem deathParticle;

    private Coroutine lifeCoroutine;

    void Awake(){
        lifeCoroutine = StartCoroutine(ProyectileLife());
    }

    private void Update()
    {
        if(!GameOrchestrator.instance.gamePaused)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private IEnumerator ProyectileLife()
    {
        yield return GameOrchestrator.instance.PausableWait(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other){
        if (string.IsNullOrEmpty(objetiveTag) || other.gameObject.CompareTag(objetiveTag))
        {
            if (lifeCoroutine != null)
                StopCoroutine(lifeCoroutine);

            LifeSystem lifeObjetive = other.GetComponent<LifeSystem>();

            if (lifeObjetive != null)
            {
                lifeObjetive.ChangeHealth(healthMod);
            }

            if(pickUpAudio!=null){
                SoundManager.instance.PlayAudioClip(pickUpAudio);
            }

            if(deathParticle!=null){
                deathParticle.transform.parent=null;
                deathParticle.Play();
            }

            Destroy(gameObject);
        }
    }
}
