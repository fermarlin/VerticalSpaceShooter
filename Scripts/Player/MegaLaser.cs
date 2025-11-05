using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class MegaLaser : MonoBehaviour
{
    // Este script gestiona el megalaser del jugador tanto la parte logica como la UI

    [Header("UI")]
    [SerializeField] private RectMask2D laserBar;   // La mascara que uso para ocultar parte de la barra segun la carga
    [SerializeField] private Image laserBarImg;     // La propia imagen que uso para representar la carga

    [Header("GameObject")]
    [SerializeField] private Animator megalaserAn;  // Animator para hacer el efecto del megalaser

    [Header("Config")]  
    [SerializeField] private float megalaserDuration = 3f;  // Duracion del mega laser
    [SerializeField] private float megalaserCooldown = 20f; // Tiempo total para cargar el laser
    [SerializeField] private int megalaserDPS = -1;         // Dano por segundo
    [SerializeField] private string objetiveTag = null;     // Tag de objetivos para recibir dano
    [SerializeField] private string bulletTag = null;       // Tag de balas para destruirlas si tocan el laser

    private bool megaLaserActive = false;                   // Si el megalaser esta activo
    private bool canShoot = false;                          // Si el laser esta listo para disparar
    private float megaLaserCharge = 0f;                     // Progreso de carga
    private float maxHeight;                                // Altura de la barra
    private SpaceshipInput controls;                        // El input system para activar el laser
    private List<LifeSystem> enemiesLifeSystems = new List<LifeSystem>(); // Enemigos dentro del area del laser

    void Awake()
    {
        // Creo los controles y pillo la altura maxima de la barra de la UI
        controls = new SpaceshipInput();
        maxHeight = laserBarImg.rectTransform.sizeDelta.y;
    }

    void Start()
    {
        // Comienza la carga
        StartCoroutine(ChargeLaser());
    }

    void OnEnable()
    {
        // Activo los controles y si pulso el boton de megalaser que lo intente activar
        controls.Spaceship.Enable();
        controls.Spaceship.MegaLaser.performed += OnMegaLaserPerformed;
    }

    void OnDisable()
    {
        // Si quito el component que no se quede colgado el input system
        controls.Spaceship.Disable();
    }

    void Update(){
        // Mientras el laser esta activo, aplica DPS por segundo a todos los objetivos dentro
        if(megaLaserActive&&enemiesLifeSystems.Count>0){
            for (int i = enemiesLifeSystems.Count - 1; i >= 0; i--)
            {
                // Si ya se ha muerto el enemigo que lo limpie
                if (enemiesLifeSystems[i] == null)
                {
                    enemiesLifeSystems.RemoveAt(i);
                    continue;
                }

                // Dano continuo
                enemiesLifeSystems[i].ChangeHealth((megalaserDPS * Time.deltaTime));
            }
        }
    }

    private void OnMegaLaserPerformed(InputAction.CallbackContext context)
    {
        // Activa el mega laser si el juego no esta pausado, no esta ya activo y esta cargado
        if (!GameOrchestrator.instance.gamePaused && !megaLaserActive && canShoot)
        {
            megalaserAn.SetBool("ActiveLaser", true);
            megaLaserActive = true;
            StartCoroutine(ActiveLaser());
        }
    }

    private void MegaLaserUIUpdate()
    {
        // Actualiza la UI modificando el padding inferior segun el porcentaje cargado
        laserBar.padding = new Vector4(0, 0, 0, (1 - megaLaserCharge / megalaserCooldown) * maxHeight);
    }

    private IEnumerator ActiveLaser()
    {
        // Consume la carga durante su duracion y luego vuelve a cargar
        float timer = 0f;
        float initialCharge = megaLaserCharge;
        while (timer < megalaserDuration)
        {
            while (GameOrchestrator.instance.gamePaused) yield return null;

            timer += Time.deltaTime;
            // Consume de la carga actual hasta 0
            megaLaserCharge = Mathf.Lerp(initialCharge, 0f, timer / megalaserDuration);
            MegaLaserUIUpdate();

            yield return null;
        }

        // Carga agotada
        megaLaserCharge = 0f;
        MegaLaserUIUpdate();

        // Desactiva el laser y reinicia ciclo de carga
        megalaserAn.SetBool("ActiveLaser", false);
        megaLaserActive = false;
        canShoot = false;

        StartCoroutine(ChargeLaser());
    }

    private IEnumerator ChargeLaser()
    {
        // Rellena la barra desde 0
        float timer = 0f;
        while (!canShoot)
        {
            while (GameOrchestrator.instance.gamePaused) yield return null;

            timer += Time.deltaTime;
            megaLaserCharge = Mathf.Lerp(0f, megalaserCooldown, timer / megalaserCooldown);
            MegaLaserUIUpdate();

            // Cuando alcanza el maximo, se considera que ya puede disparar
            if (megaLaserCharge >= megalaserCooldown)
            {
                megaLaserCharge = megalaserCooldown;
                canShoot = true;
            }

            yield return null;
        }
        // Aviso al manager para que avise al jugador por mensaje en pantalla que puede usar el mega laser
        GameOrchestrator.instance.MegaLaserRecharged();
    }

    public bool UsingMLaser(){
        // Este bool publico es para que no me permita disparar si estoy usando el mega laser
        return megaLaserActive;
    }

    void OnTriggerEnter(Collider other)
    {
        // Si entra un objetivo se anade a la lista para aplicar DPS
        if (other.gameObject.CompareTag(objetiveTag))
        {
            enemiesLifeSystems.Add(other.GetComponent<LifeSystem>());
        }

        // Si entra una bala se destruye, esto es para que el jugador no se coma balas que no ve siquiera
        if (other.gameObject.CompareTag(bulletTag)){
            Destroy(other.gameObject);
        }

    }

    void OnTriggerExit(Collider other)
    {
        // Al salir un objetivo valido, se elimina de la lista para no seguir aplicando dano
        if (other.gameObject.CompareTag(objetiveTag))
        {
            enemiesLifeSystems.Remove(other.GetComponent<LifeSystem>());
        }
    }
}
