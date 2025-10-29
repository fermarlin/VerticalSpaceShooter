using UnityEngine;

public class SpaceshipWeapon : MonoBehaviour
{
    [SerializeField]
    private SpaceshipShoot rightWeapon;
    [SerializeField]
    private SpaceshipShoot centralWeapon;
    [SerializeField]
    private SpaceshipShoot leftWeapon;
    [SerializeField]
    private Animator playerAnimator;

    public void DoubleWeapon(){
        playerAnimator.SetBool("DoubleWeapon",true);
        rightWeapon.enabled=true;
        centralWeapon.enabled=false;
        leftWeapon.enabled=true;
        GameOrchestrator.instance.DoubleWeapon();
    }
    public void SimpleWeapon(){
        playerAnimator.SetBool("DoubleWeapon",false);
        rightWeapon.enabled=false;
        centralWeapon.enabled=true;
        leftWeapon.enabled=false;
    }
}
