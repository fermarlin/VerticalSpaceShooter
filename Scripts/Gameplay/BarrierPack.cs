using UnityEngine;

public class BarrierPack : MonoBehaviour
{
    // Este script gestiona el power-up de barrera

    [SerializeField]
    private string objetiveTag = null; // Tag del objetivo que puede recogerlo

    void OnTriggerEnter(Collider other){
        // Si no hay tag objetivo o el objeto que entra coincide con ese tag se activa
        if (string.IsNullOrEmpty(objetiveTag) || other.gameObject.CompareTag(objetiveTag)){
            
            // Buscamos LifeSystem en el objeto que choco
            LifeSystem lifeObjetive = other.GetComponent<LifeSystem>();

            if (lifeObjetive != null)
            {
                // Activa la barrera en el objetivo, ahi dentro ya me aseguro si se puede o no dentro del propio lifesystem
                lifeObjetive.AddBarrier();
            }
        }
    }
}
