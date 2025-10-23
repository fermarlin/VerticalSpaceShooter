using UnityEngine;
using System.Collections;

public class SimpleEnemyLogic : MonoBehaviour
{
    [SerializeField]
    private float vSpeed = 5;
    [SerializeField]
    private float maxHspeed = 3;
    [SerializeField]
    private float lifeTime = 3;


    private Coroutine lifeCoroutine;
    private float hSpeed;

    void Awake(){
        lifeCoroutine = StartCoroutine(EnemyLife());
        bool randomDir = Random.value > 0.5f;
        hSpeed = Random.Range(0, maxHspeed);
        if (randomDir)
        {
            hSpeed = -hSpeed; 
        }
        
    }

    private void Update()
    {
        if(!GameOrchestrator.instance.gamePaused)
        transform.Translate((Vector3.forward * -vSpeed+Vector3.right*hSpeed) * Time.deltaTime);
    }

    private IEnumerator EnemyLife()
    {
        yield return GameOrchestrator.instance.PausableWait(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other){
        StopCoroutine(lifeCoroutine);
        LifeSystem lifeObjetive = other.GetComponent<LifeSystem>();
        LifeSystem selfLife = GetComponent<LifeSystem>();

        if (lifeObjetive != null)
        {
            lifeObjetive.ChangeHealth(-1);
        }

        if(other.CompareTag("Player")){
            selfLife.AutoDestruction();
        }


    }
}
