using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Range(0f, 0.5f)] private float screenMargin = 0.05f;
    [SerializeField, Min(0f)] private float cooldown = 2f;

    private Camera cam;
    private bool isRunning = true;
    private float depth;
    private float[] worldLimits = new float[2];
    
    void Start()
    {
        cam = Camera.main;
        depth = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);
        Vector3 topPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, depth));
        transform.position = topPos;

        Vector3 leftWorld  = cam.ViewportToWorldPoint(new Vector3(screenMargin, 0f, depth));
        Vector3 rightWorld = cam.ViewportToWorldPoint(new Vector3(1f - screenMargin, 0f, depth));
        worldLimits[0] = leftWorld.x;
        worldLimits[1] = rightWorld.x;

        StartCoroutine(SpawnLoop());
    }


    private IEnumerator SpawnLoop()
    {
        while (isRunning)
        {
            while (GameOrchestrator.instance.gamePaused) yield return null;

            SpawnEnemy(worldLimits[0], worldLimits[1]);

            yield return GameOrchestrator.instance.PausableWait(cooldown);
        }
    }

    private void SpawnEnemy(float minX, float maxX)
    {
            float randomX = Random.Range(minX, maxX);
            Vector3 pos = new Vector3(randomX, transform.position.y, transform.position.z);
            Instantiate(enemyPrefab, pos, transform.rotation);
    }

    private void OnDisable()
    {
        isRunning = false;
    }
}
