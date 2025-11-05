using UnityEngine;
using System.Collections;

public class SimpleEnemyLogic : MonoBehaviour
{
    // Este script gestiona la logica del meteorito, el enemigo simple que baja por la pantalla y ya

    [SerializeField]
    private float vSpeed = 5;           // Velocidad vertical
    [SerializeField]
    private float maxHspeed = 3;        // Velocidad horizontal maxima
    [SerializeField]
    private float lifeTime = 3;         // Tiempo de vida antes de destruirse

    private Coroutine lifeCoroutine;     // Referencia a la corrutina de vida para poder pararla cuando colisiona
    private float hSpeed;                // Velocidad horizontal real

    void Awake(){
        // Activo la corutina de vida para que se destruya con el tiempo y que no haya muchos meteoritos en escena que ya ni se vean
        lifeCoroutine = StartCoroutine(EnemyLife());

        // Pongo aleatoria la direccion y la magnitud del meteorito
        bool randomDir = Random.value > 0.5f;
        hSpeed = Random.Range(0, maxHspeed);
        if (randomDir)
        {
            hSpeed = -hSpeed; 
        }
        
    }

    private void Update()
    {
        // Movimiento del enemigo, como siempre si no esta el juego pausado
        if(!GameOrchestrator.instance.gamePaused)
        transform.Translate((Vector3.forward * -vSpeed+Vector3.right*hSpeed) * Time.deltaTime);
    }

    private IEnumerator EnemyLife()
    {
        // Cuando pasa el tiempo destruye el objeto
        yield return GameOrchestrator.instance.PausableWait(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other){
        // Al tocar otro objeto quita la corutina de vida
        StopCoroutine(lifeCoroutine);

        // Intenta aplicar dano al objetivo si tiene LifeSystem
        LifeSystem lifeObjetive = other.GetComponent<LifeSystem>();
        LifeSystem selfLife = GetComponent<LifeSystem>();

        if (lifeObjetive != null)
        {
            lifeObjetive.ChangeHealth(-1);
        }

        // Si choca con Player o Enemy, este enemigo se autodestruye
        if(other.CompareTag("Player")||other.CompareTag("Enemy")){
            selfLife.AutoDestruction();
        }
    }
    
}
