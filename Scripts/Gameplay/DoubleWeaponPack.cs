using UnityEngine;

public class DoubleWeaponPack : MonoBehaviour
{
    [SerializeField]
    private string objetiveTag = null; 

    void OnTriggerEnter(Collider other){
        
        if (string.IsNullOrEmpty(objetiveTag) || other.gameObject.CompareTag(objetiveTag)){
               
            SpaceshipWeapon spaceshipWeapon = other.GetComponent<SpaceshipWeapon>();

            if (spaceshipWeapon != null)
            {
                spaceshipWeapon.DoubleWeapon();
            } 
        }
    }
}
