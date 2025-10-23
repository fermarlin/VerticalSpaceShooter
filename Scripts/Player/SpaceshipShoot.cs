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

    private SpaceshipInput controls;
    private bool isShooting=false;
    private bool canShoot = true;

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
        while (isShooting && canShoot)
        {
            if(shootAudio!=null) SoundManager.instance.PlayAudioClip(shootAudio);
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            canShoot = false;
            yield return new WaitForSeconds(fireRate);
            canShoot = true;
        }
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        if(!GameOrchestrator.instance.gamePaused){
            isShooting = true;
            StartCoroutine(ShootLoop());
        }
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        isShooting = false;
    }
}
