using UnityEngine;
using System.Collections;

public class DistanceEnemyLogic : MonoBehaviour
{
    [SerializeField]
    private float vSpeed = 5;
    [SerializeField]
    private float hSpeed = 3;

    void Awake(){
       
    }

    private void Update()
    {
        if(!GameOrchestrator.instance.gamePaused)
        transform.Translate((Vector3.forward * -vSpeed+Vector3.right*hSpeed) * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other){
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
