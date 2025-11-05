using UnityEngine;

public class DoubleWeaponPack : MonoBehaviour
{
    // Este script se encarga de activar el arma doble, funciona igual que el de la barrera

    [SerializeField]
    private string objetiveTag = null; // Tag del objeto que puede recogerlo

    void OnTriggerEnter(Collider other){
        // Si no se exige tag o el que entra coincide, intentamos activar los canones
        if (string.IsNullOrEmpty(objetiveTag) || other.gameObject.CompareTag(objetiveTag)){
               
            SpaceshipWeapon spaceshipWeapon = other.GetComponent<SpaceshipWeapon>();

            if (spaceshipWeapon != null)
            {
                // Activa el doble disparo en la nave
                spaceshipWeapon.DoubleWeapon();
            } 
        }
    }
}
