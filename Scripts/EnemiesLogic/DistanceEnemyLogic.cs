using UnityEngine;
using System.Collections;

public class DistanceEnemyLogic : MonoBehaviour
{
    [SerializeField]
    private float vSpeed = 5;
    [SerializeField]
    private float hSpeed = 3;
    [SerializeField]
    private float descendTime = 1;
    [SerializeField]
    private float horizontalMovTime = 1;
    [SerializeField]
    private float fireRate = 0.5f;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform enemyMuzzle;
    [SerializeField]
    private AudioClip shootAudio; 

    private bool descending = true;
    private bool isRunning = true;

    void Start(){
        StartCoroutine(EnemyDescendFinish());
        StartCoroutine(EnemyInvertMovement());

    }    
    
    private void Update()
    {
        if(GameOrchestrator.instance.gamePaused) return;

        if(descending){
            transform.Translate(Vector3.forward * -vSpeed * Time.deltaTime);
        }else{
            transform.Translate(Vector3.right * hSpeed * Time.deltaTime);
        }
    }

    private IEnumerator EnemyDescendFinish()
    {
        yield return GameOrchestrator.instance.PausableWait(descendTime);
        descending=false;
        StartCoroutine(Shooting());
    }

    private IEnumerator EnemyInvertMovement()
    {
        while(isRunning){
            yield return GameOrchestrator.instance.PausableWait(horizontalMovTime);
            hSpeed=-hSpeed;
        }
    }

    private IEnumerator Shooting()
    {
        while(isRunning){
            if(shootAudio!=null) SoundManager.instance.PlayAudioClip(shootAudio);
            Instantiate(bulletPrefab, enemyMuzzle.transform.position, enemyMuzzle.transform.rotation);
            yield return GameOrchestrator.instance.PausableWait(fireRate);
        }
    }


    private void OnDisable()
    {
        isRunning = false;
    }
}
