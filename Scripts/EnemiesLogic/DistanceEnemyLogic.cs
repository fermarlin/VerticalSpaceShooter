using UnityEngine;
using System.Collections;

public class DistanceEnemyLogic : MonoBehaviour
{
    //Este script se encarga de gestionar la logica del enemigo que baja y despues se pone a disparar 

    [SerializeField]
    private float vSpeed = 5;                  // Velocidad de descenso
    [SerializeField]
    private float hSpeed = 3;                  // Velocidad de movimiento lateral
    [SerializeField]
    private float descendTime = 1;             // Tiempo que pasa descendiendo antes de empezar a moverse lateralmente
    [SerializeField]
    private float horizontalMovTime = 1;       // Intervalo para invertir la direccion horizontal
    [SerializeField]
    private float fireRate = 0.5f;             // Cadencia de disparo
    [SerializeField]
    private GameObject bulletPrefab;           // Prefab de bala
    [SerializeField]
    private Transform enemyMuzzle;             // Punto de salida del disparo
    [SerializeField]
    private AudioClip shootAudio;              // Sonido de disparo

    private bool descending = true;            // Bool que gestiona si esta bajando o no
    private bool isRunning = true;             // Este bool es para que gestionar si el enemigo esta funcionando o no

    void Start(){
        // Esto es igual que el boss, empieza bajando y llamo a la corutina que le dice al enemigo que deje de bajar, la otra es para que vaya invirtiendo el movimiento del enemigo cada tanto tiempo para hacer el efecto de space invaders
        StartCoroutine(EnemyDescendFinish());
        StartCoroutine(EnemyInvertMovement());

    }    
    
    private void Update()
    {
        // Que el enemigo no se mueva si el juego esta pausado
        if(GameOrchestrator.instance.gamePaused) return;

        if(descending){
            // Primero que vaya bajando
            transform.Translate(Vector3.forward * -vSpeed * Time.deltaTime);
        }else{
            // Despues que se mueva de derecha a izquierda
            transform.Translate(Vector3.right * hSpeed * Time.deltaTime);
        }
    }

    private IEnumerator EnemyDescendFinish()
    {
        // Al terminar el descenso, pasa a moverse lateralmente y empieza a disparar
        yield return GameOrchestrator.instance.PausableWait(descendTime);
        descending=false;
        StartCoroutine(Shooting());
    }

    private IEnumerator EnemyInvertMovement()
    {
        // Invierte la direccion horizontal mientras siga activo
        while(isRunning){
            yield return GameOrchestrator.instance.PausableWait(horizontalMovTime);
            hSpeed=-hSpeed;
        }
    }

    private IEnumerator Shooting()
    {
        // Disparo periodico mientras este activo
        while(isRunning){
            if(shootAudio!=null) SoundManager.instance.PlayAudioClip(shootAudio);
            Instantiate(bulletPrefab, enemyMuzzle.transform.position, enemyMuzzle.transform.rotation);
            yield return GameOrchestrator.instance.PausableWait(fireRate);
        }
    }


    private void OnDisable()
    {
        // Al desactivarse el objeto, corta los bucles
        isRunning = false;
    }
}
