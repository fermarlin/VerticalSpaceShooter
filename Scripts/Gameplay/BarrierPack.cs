using UnityEngine;

public class BarrierPack : MonoBehaviour
{
    [SerializeField]
    private string objetiveTag = null; 

    void OnTriggerEnter(Collider other){
        
        if (string.IsNullOrEmpty(objetiveTag) || other.gameObject.CompareTag(objetiveTag)){

            LifeSystem lifeObjetive = other.GetComponent<LifeSystem>();

            if (lifeObjetive != null)
            {
                
                    lifeObjetive.AddBarrier();
                
            }
        }
    }
}
