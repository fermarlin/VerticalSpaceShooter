using UnityEngine;
using System.Collections;

public class FirstBossLogic : MonoBehaviour
{
    // Este script gestiona la logica del jefe

    [SerializeField] private float hSpeed = 3f;                 // Velocidad horizontal para ir de un lado a otro 
    [SerializeField] private float screenMargin = 0.05f;        // Margen de pantalla para que el boss no vaya exactamente al punto de la pantalla porque si no va a salirse de pantalla
    [SerializeField] private float descendTime = 1;             // Tiempo de descenso antes de empezar a moverse en el eje horizontal
    [SerializeField] private float vSpeed = 5;                  // Velocidad de descenso
    [SerializeField] private float lateralFireRate = 0.5f;      // Cadencia de disparo para los canones laterales
    [SerializeField] private float centralFireRate = 0.7f;      // Cadencia de disparo para el canon central
    [SerializeField] private GameObject bulletPrefab;           // Prefab de bala para el canon central
    [SerializeField] private GameObject bulletSecondPrefab;     // Prefab de bala para los canones laterales
    [SerializeField] private Transform[] enemyMuzzles;          // El punto donde van a spawnear las balas
    [SerializeField] private AudioClip shootAudio;              // Audio de disparo
    [SerializeField] private ParticleSystem[] smokeParticles;   // Particulas de humo para que se refleje como baja la vida
    [SerializeField] private LifeSystem bossHealth;             // Sistema de vida del jefe

    private bool descending = true;                             // Booleano para gestionar si esta descendiendo 
    private bool isRunning = true;                              // Controla los bucles de disparo
    private Camera cam;                                         // Referencia a la camara principal
    private float depth;                                        // Profundidad del jefe respecto a la camara, asi puedo saber los limites de la pantalla
    private float[] worldLimits = new float[2];                 // Limites del mundo en X

    void Start()
    {
        // Pillo la camara y calcula la profundidad para convertir de viewport a mundo
        cam = Camera.main;
        depth = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);

        // Calcula los limites laterales en X usando el margen de pantalla
        Vector3 leftWorld  = cam.ViewportToWorldPoint(new Vector3(screenMargin, 0f, depth));
        Vector3 rightWorld = cam.ViewportToWorldPoint(new Vector3(1f - screenMargin, 0f, depth));
        worldLimits[0] = Mathf.Min(leftWorld.x, rightWorld.x);
        worldLimits[1] = Mathf.Max(leftWorld.x, rightWorld.x);

        // Suscribo los cambios de vida para activar el humo en funcion de la vida que le quede
        bossHealth.OnHealthChanged+=healthChecker;

        // Tras el tiempo de descenso, empieza a moverse en horizontal
        StartCoroutine(EnemyDescendFinish());

    }

    void Update()
    {
        // Esto es para no mover el boss si el juego esta pausado
        if (GameOrchestrator.instance.gamePaused) return;
        
        if(descending){
            // Esto es para bajar el boss y ponerlo a una distancia del player
            transform.Translate(Vector3.forward * -vSpeed * Time.deltaTime);
        }else{
                // Si ya no esta descendiendo que se mueva al borde
                Vector3 p = transform.position;
                p.x += hSpeed * Time.deltaTime;

                if (p.x <= worldLimits[0] || p.x >= worldLimits[1])
                {
                    // Limita a los bordes y cambia el sentido de la velocidad
                    p.x = Mathf.Clamp(p.x, worldLimits[0], worldLimits[1]);
                    hSpeed = -hSpeed;
                }

                transform.position = p;
            }

    }

    private void healthChecker(float currentHealth, float maxHealth)
    {
        // Activa particulas de humo al perder 25%, 50% y 75% de la vida
        if (maxHealth <= 0f) return;

        float lostPct = (maxHealth - currentHealth) / maxHealth; 

        if ( lostPct >= 0.25f)
        {
            smokeParticles[0].Play();
        }
        if ( lostPct >= 0.50f)
        {
            smokeParticles[1].Play();
        }
        if ( lostPct >= 0.75)
        {
            smokeParticles[2].Play();
        }
    }

    private IEnumerator EnemyDescendFinish()
    {
        // Un temporizador para que deje de bajar cuando pase el tiempo que le hemos dicho que tenga
        yield return GameOrchestrator.instance.PausableWait(descendTime);
        descending=false;

        // Inicia las corrutinas de disparo para cada muzzle disponible
        if (enemyMuzzles != null)
        {
            if (enemyMuzzles.Length > 0) StartCoroutine(Shooting(enemyMuzzles[0], lateralFireRate, bulletSecondPrefab));
            if (enemyMuzzles.Length > 1) StartCoroutine(Shooting(enemyMuzzles[1], centralFireRate, bulletPrefab));
            if (enemyMuzzles.Length > 2) StartCoroutine(Shooting(enemyMuzzles[2], lateralFireRate, bulletSecondPrefab));
        }
    }

    private IEnumerator Shooting(Transform muzzle, float fireRate, GameObject bulletPre)
    {
        // Bucle de disparo, como todo lo que tiene PausableWait no va a ejecutarse si el juego esta en pausa
        while (isRunning)
        {
            if (shootAudio != null) SoundManager.instance.PlayAudioClip(shootAudio);
            Instantiate(bulletPre, muzzle.position, muzzle.rotation);
            yield return GameOrchestrator.instance.PausableWait(fireRate);
        }
    }

    private void OnDisable(){
        isRunning = false;     // Al desactivarse, corta el bucle de disparo

    } 
}
