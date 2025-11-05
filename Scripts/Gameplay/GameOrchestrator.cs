using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameOrchestrator : MonoBehaviour
{
    [Header("Game State")]
    // Gestiona el nivel, mensajes del robot y instaciar el boss principalmente
    public bool gamePaused = false;                     // Con este bool gestiono la pausa en todo el juego
    public static GameOrchestrator instance;            // Con esto puedo acceder al GameOrchestrator sin tener que asignarlo por el inspector ni tener que buscarlo por tags ni nada, como solo va a haber uno no hay problema
    public float resetTime = 5f;                        // Segundos de espera para cambiar el juego

    //Esta estructura la uso para mandarle al TextBoxManager lo que quiero que diga en cada momento
    [System.Serializable]
    public struct TextPremade
    {
        public string textPremade;                      // Texto para el TextBoxManager
        public AudioClip audioC;                        // Audio asociado al texto
    }

    [SerializeField] private Animator cameraAnimator;  // Animator de la camara, asi puedo moverla mientras el jugador juega, en una cinematica generalmente

    [Header("Texts")]
    [SerializeField] private TextPremade[] initTexts;               // Mensajes cuando empieza la mision
    [SerializeField] private TextPremade[] shieldUpText;            // Mensajes cuando consigues el escudo
    [SerializeField] private TextPremade[] shieldDownText;          // Mensajes cuando pierdes el escudo
    [SerializeField] private TextPremade[] megaLaserActiveText;     // Mensajes cuando el megalaser esta listo
    [SerializeField] private TextPremade[] doubleWeaponActiveText;  // Mensajes para cuando el player consigue la mejora de doble disparo
    [SerializeField] private TextPremade[] bossText;                // Mensajes cuando el jefe spawnea
    [SerializeField] private TextPremade[] deadPlayerText;          // Mensajes cuando el jugador muere
    [SerializeField] private TextPremade[] missionCompleteText;     // Mensajes para cuando el jugador muere
    [SerializeField] private Animator pauseAnimator;                 // Animator de la pantalla de pausa, para activarlo o no cuando toque

    [Header("Boss")]
    [SerializeField] private float survivalTime = 0;   // Segundos antes de que el jefe spawnee
    [SerializeField] private GameObject bossprefab;    // Prefab del jefe final
    [SerializeField] private GameObject enemies;       // El padre de los spawneers y por consiguiente los enemigos, asi cuando llegue el boss los puedo ocultar y que no molesten en la batalla


    private bool cinematic = false;                    // Este bool es como el de pausa, pero es una pausa que va por encima de lo que pueda hacer el jugador, asi puedo pausar el juego y aunque el player intente abusar del menu de pausa no pasara nada
    private bool hasGotAnyBarrier = false;             // Para no abrumar al jugador cada vez que consigue un escudo, que este la primera vez y ya
    private bool hasGotDoubleWeapon = false;           // Lo mismo que el bool de la barrera, para que solo suene la primera vez, ademas que es una mejora que el jugador no pierde a menos que muera
    private Coroutine textCoroutine;                   // Aqui guardo la corutina para poder cortarlo en cualquier momento si hay un nuevo mensaje
    private SpaceshipInput controls;                   // El input system del jugador
    private bool playerIsDead = false;                 // Para que si el jugador esta muerto por ejemplo que no spawnee el boss

    void Awake()
    {
        // Inicializa el game orchestrator para que pueda acceder donde quiera y creo los controles para despues asignarle la pausa
        if (instance == null) instance = this;
        controls = new SpaceshipInput();
    }

    void OnEnable()
    {
        // Activo los controles y asigno la funcion de pausa
        controls.Spaceship.Enable();
        controls.Spaceship.Pause.performed += OnPausePerformed;
    }

    void OnDisable()
    {
        // Esto deshabilita los controles, para que no se quede nada colgando
        controls.Spaceship.Disable();
    }

    void Start()
    {
        // Mensaje inicial y que empiece a contar el tiempo hasta que spawnee el boss
        ShowRandom(initTexts);
        StartCoroutine(LevelTimer());
    }

    void ShowRandom(TextPremade[] pool)
    {
        // Esta funcion permite usar un texto random de entre los posibles del array que tiene cada uno de ellos
        if (pool == null || pool.Length == 0) return; // Si no hay ningun valor que salga, para que no haya problemas
        if (textCoroutine != null) StopCoroutine(textCoroutine); //Para el audio que hubiera
        var i = Random.Range(0, pool.Length); //Elige un valor entre los posibles
        textCoroutine = StartCoroutine(TextBoxManager.instance.ChangeTextCoroutine(pool[i].textPremade, pool[i].audioC)); //Llama a la funcion de Textboxmanager que se encarga de esto
    }

    public void CinematicPause(bool value)
    {
        // Esta funcion pausa el juego como he comentado antes, por encima del pausa normal
        cinematic = value;
        PauseGame(value);
    }

    public void PauseGame(bool value)
    {
        // Pone/quita pausa. Si hay una cinematica se fuerza la pausa
        if(cinematic){
            gamePaused = true;
        }else gamePaused = value;
        
        // El Animator de pausa solo se activa si no esta pausado por una cinematica
        bool activePauseMenu = false;

        if(!cinematic&&value){
            activePauseMenu= true;
        }

        pauseAnimator.SetBool("GamePause", activePauseMenu);
    }

    public void Barrier(bool up)
    {
        // Un callback para cuando el jugador coge una mejora que le da la barrera
        if (up && !hasGotAnyBarrier)
        {
            ShowRandom(shieldUpText);
            hasGotAnyBarrier = true;
        }
        else if (!up)
        {
            ShowRandom(shieldDownText);
        }
    }

    public void MegaLaserRecharged()
    {
        // Aviso cuando el megalaser esta listo
        ShowRandom(megaLaserActiveText);
    }

    public void DoubleWeapon()
    {
        // Aviso cuando consigues el doble canon
        if (hasGotDoubleWeapon) return;
        ShowRandom(doubleWeaponActiveText);
        hasGotDoubleWeapon = true;
    }

    public void PlayerDead()
    {
        // Esta funcion gestiona cuando el jugador ha muerto, pone la bandera llama a resetear la escena y pone un mensaje en pantalla para mientras respawnea el jugador
        playerIsDead = true;
        StartCoroutine(ResetScene());
        ShowRandom(deadPlayerText);
    }

    void BossDead(float currentHealth, float maxHealth)
    {
        // Callback de vida del jefe, cuando le hacemos dano se llama a esta funcion y si su vida llega a 0 se le dice a la camara que haga la animacion de boss derrotado, pongo el texto en pantalla y reseteo al menu
        if (currentHealth > 0) return;
        cameraAnimator.SetBool("BossDead", true);
        ShowRandom(missionCompleteText);
        StartCoroutine(ResetToMainMenu());
    }

    public IEnumerator ResetScene()
    {
        // Reinicia la escena actual despues de esperar el tiempo que le pongamos por el inspector, le pregunto al motor en que escena estamos y llamo el reset a esta escena
        int index = SceneManager.GetActiveScene().buildIndex;
        yield return PausableWait(resetTime);
        LoadManager.instance.LoadScene(index);
    }

    public IEnumerator ResetToMainMenu()
    {
        // Lo mismo que ResetScene solo que esta va al menu principal
        yield return new WaitForSeconds(resetTime);
        LoadManager.instance.LoadScene(0);
    }

    public IEnumerator PausableWait(float seconds)
    {
        // Esta funcion es clave, me permite pausar el juego sin poner el Time.deltaTime a 0, y es muy simple, si el juego esta pausado no se suma el tiempo que esta pasando (Time.deltaTime)
        float timepassed = 0f;
        while (timepassed < seconds)
        {
            if (!gamePaused) timepassed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator LevelTimer()
    {
        // Esta funcion gestiona cuando se spawneea el boss
        yield return PausableWait(survivalTime); // Espero el tiempo que hayamos establecido en el inspector
        if (playerIsDead) yield break;            // Si el jugador ha muerto, no se genera el jefe

        enemies.SetActive(false);                 // Oculta el grupo de enemigos
        ShowRandom(bossText);                     // Texto para cuando el jefe spawneea

        var bossSpawned = Instantiate(bossprefab);                             // Instancia el jefe
        bossSpawned.GetComponent<LifeSystem>().OnHealthChanged += BossDead;    // Suscribo el evento de cambio de vida a la funcion que esta mas arriba de cuando el boss muere
        cameraAnimator.SetBool("Boss", true);                                  // Activa animacion de la camara del jefe
    }

    void OnPausePerformed(InputAction.CallbackContext context)
    {
        // Esto es para que cuando pulsemos escape o el boton de pausa del mando pause/despause el juego
        PauseGame(!gamePaused);
    }
}
