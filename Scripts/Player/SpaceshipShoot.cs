using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpaceshipShoot : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float fireRate = 0.3f;
    [SerializeField]
    private AudioClip shootAudio; 
    [SerializeField]
    private MegaLaser megaLaser; 

    private SpaceshipInput controls;
    private bool isShooting=false;

    void Awake() {
       controls = new SpaceshipInput(); 
    } 

    void OnEnable()
    {
        controls.Spaceship.Enable();

        controls.Spaceship.Shoot.performed += OnShootPerformed;
        controls.Spaceship.Shoot.canceled += OnShootCanceled;
    }
    
    void OnDisable(){
        controls.Spaceship.Disable();
    } 

    private IEnumerator ShootLoop()
    {
        while (isShooting)
        {
            if (GameOrchestrator.instance.gamePaused || (megaLaser != null && megaLaser.UsingMLaser()))
            {
                yield return null;
                continue;
            }

            if (shootAudio != null)
                SoundManager.instance.PlayAudioClip(shootAudio);

            Instantiate(bulletPrefab, transform.position, transform.rotation);

            yield return new WaitForSeconds(fireRate);
        }
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        if (GameOrchestrator.instance.gamePaused) return;
        if (megaLaser != null && megaLaser.UsingMLaser()) return;
        if (isShooting) return; 

        isShooting = true;
        StartCoroutine(ShootLoop());
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        isShooting = false;
    }
}
