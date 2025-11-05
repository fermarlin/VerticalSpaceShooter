using UnityEngine;

public class SpaceshipWeapon : MonoBehaviour
{
    // Este script gestiona las armas del player

    [SerializeField]
    private SpaceshipShoot rightWeapon;     // Canon derecho 
    [SerializeField]
    private SpaceshipShoot centralWeapon;   // Canon central
    [SerializeField]
    private SpaceshipShoot leftWeapon;      // Canon izquierdo
    [SerializeField]
    private Animator playerAnimator;        // Animator del jugador

    public void DoubleWeapon(){
        // Activa el modo doble
        playerAnimator.SetBool("DoubleWeapon",true);
        rightWeapon.enabled=true;
        centralWeapon.enabled=false;
        leftWeapon.enabled=true;
        GameOrchestrator.instance.DoubleWeapon();
    }
    public void SimpleWeapon(){
        // Vuelve al modo simple
        playerAnimator.SetBool("DoubleWeapon",false);
        rightWeapon.enabled=false;
        centralWeapon.enabled=true;
        leftWeapon.enabled=false;
    }
}
