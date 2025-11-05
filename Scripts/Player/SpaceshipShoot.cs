using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpaceshipShoot : MonoBehaviour
{
    // Gestiona el disparo del jugador

    [SerializeField]
    private GameObject bulletPrefab;          // Prefab de la bala
    [SerializeField]
    private float fireRate = 0.3f;            // Cadencia entre disparos
    [SerializeField]
    private AudioClip shootAudio;             // Sonido de disparo
    [SerializeField]
    private MegaLaser megaLaser;              // Referencia al MegaLaser para bloquear disparo normal cuando se usa

    private SpaceshipInput controls;          // El inputsystem del jugador
    private bool isShooting=false;            // Para saber si estas manteniendo el disparo
    private bool canshoot=true;               // Esto es para evitar disparos antes de tiempo

    void Awake() {
       // Inicializa el mapa de controles
       controls = new SpaceshipInput(); 
    } 

    void OnEnable()
    {
        // Habilita el  y suscripciones a los eventos de disparo
        controls.Spaceship.Enable();

        controls.Spaceship.Shoot.performed += OnShootPerformed; // Esto para que cuando se pulse el boton de disparo se llama a la funcion que se encarga de iniciar el bucle de disparo
        controls.Spaceship.Shoot.canceled += OnShootCanceled;   // Aqui para cuando se suelte el boton
    }
    
    void OnDisable(){
        // Deshabilita los controles
        controls.Spaceship.Disable();
    } 

    private IEnumerator ShootLoop()
    {
        // Bucle de disparo mientras isShooting este activo
        while (isShooting)
        {
            // Si el juego esta en pausa, el MegaLaser esta activo o aun no ha terminado el tiempo de cadencia que espere un poco
            if (GameOrchestrator.instance.gamePaused || (megaLaser != null && megaLaser.UsingMLaser())||!canshoot)
            {
                yield return null;
                continue;
            }

            // Se llama al soundmanager para que suene el audio de disparo
            if (shootAudio != null)
                SoundManager.instance.PlayAudioClip(shootAudio);

            // Instancia la bala en la posicion y rotacion actuales del muzzle de la nave
            Instantiate(bulletPrefab, transform.position, transform.rotation);

            // Bloquea los nuevos disparos hasta cumplir fireRate
            canshoot=false;
            yield return new WaitForSeconds(fireRate);
            canshoot=true;
        }
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        // Cuando pulso el boton de disparo checkeo que el juego no esta pausado o no este activo el MegaLaser
        if (GameOrchestrator.instance.gamePaused) return;
        if (megaLaser != null && megaLaser.UsingMLaser()) return;
        if (isShooting) return; //Por supuesto si ya estaba disparando que no dispare de nuevo

        isShooting = true;//Digo que estoy disparando y empieza el loop de disparo
        StartCoroutine(ShootLoop());
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        // Al soltar el boton se corta el bucle de disparo
        isShooting = false;
    }
}
