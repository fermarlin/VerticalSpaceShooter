using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour
{
    // Este script gestiona como se van generando los enemigos

    [SerializeField] 
    private GameObject enemyPrefab;                       // Prefab del enemigo a instanciar
    [SerializeField, Range(0f, 0.5f)] 
    private float screenMargin = 0.05f;                   // Margen lateral para no spawnear pegado al borde
    [SerializeField, Min(0f)] 
    private float cooldown = 2f;                          // Tiempo entre spawns
    [SerializeField, Min(1)] 
    private int maxSimultaneousEnemies = 10;              // Maximo de enemigos activos

    private Camera cam;                                   // Camara principal para convertir de viewport a mundo
    private bool isRunning = true;                        // Control para el bucle del spawn
    private float depth;                                  // Profundidad del generador respecto a la camara (para los limites del viewport)
    private float[] worldLimits = new float[2];           // Limites de X en mundo [izquierda, derecha]

   
    void Start()
    {
        // Pillo la camara y calcula profundidad para hacer ViewportToWorldPoint
        cam = Camera.main;
        depth = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);

        // Situo el generador en la parte superior de la pantalla
        Vector3 topPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, depth));
        transform.position = topPos;

        // Calcula los limites laterales en X con margen, igual que el boss, para que no se spawneen justo en el borde
        Vector3 leftWorld  = cam.ViewportToWorldPoint(new Vector3(screenMargin, 0f, depth));
        Vector3 rightWorld = cam.ViewportToWorldPoint(new Vector3(1f - screenMargin, 0f, depth));
        worldLimits[0] = leftWorld.x;
        worldLimits[1] = rightWorld.x;

        // Arranca el bucle de spawns
        StartCoroutine(SpawnLoop());
    }


    private IEnumerator SpawnLoop()
    {
        // Spawnea enemigos siempre que el juego no este pausado o no este en cooldown la instanciacion de enemigos
        while (isRunning)
        {
            while (GameOrchestrator.instance.gamePaused) yield return null;

            SpawnEnemy(worldLimits[0], worldLimits[1]);

            yield return GameOrchestrator.instance.PausableWait(cooldown);
        }
    }

    private void SpawnEnemy(float minX, float maxX)
    {
        // Crea un nuevo enemigo si no se ha alcanzado el maximo, para saber si he llegado al maximo los pongo como hijos y pregunto el numero de hijos que tiene el generador
        if (this.transform.childCount < maxSimultaneousEnemies)
        {
            float randomX = Random.Range(minX, maxX);
            Vector3 pos = new Vector3(randomX, transform.position.y, transform.position.z);
            GameObject newEnemy = Instantiate(enemyPrefab, pos, transform.rotation);
            newEnemy.transform.SetParent(this.transform);
        }
    }

    private void OnDisable()
    {
        // Al desactivarse el generador, detiene el bucle
        isRunning = false;
    }
}
