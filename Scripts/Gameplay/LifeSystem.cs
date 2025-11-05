using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    // Este script gestiona el sistema de vida de los enemigos

    [Header("Health Parameters")]
    [SerializeField]
    private int maxHealth = 2;                      // Vida maxima de la nave
    [SerializeField] 
    private ParticleSystem deathParticle;           // Particulas para cuando muere
    [SerializeField] 
    private Animator shipAnimator;                  // Animator para dar feedback de dano

    [Header("Power UP Parameters")]
    [SerializeField]
    [Range(0, 1)]
    private float dropProb = .5f;                   // Probabilidad de dropear un objeto
    [SerializeField]
    private GameObject[] poweUpPrefab;              // Lista de posibles power-ups a dropear cuando mueres

    [Header("Barrier Parametres")]
    [SerializeField]
    private bool canHaveBarrier = false;            // Si este objeto puede activar el escudo 
    [SerializeField]
    private int barriermaxHealth = 2;               // Cuantos golpes puede recibir el escudo
    [SerializeField]
    private Animator barrier;                       // Animator de el escudo
    
    [Header("Game Orchestrator Parametres")]
    [SerializeField]
    private bool isPlayer = false;                  // Marca si este LifeSystem es el jugador

    private bool activeBarrier = false;             // Si el escudo esta activa o no
    private float currentbarrierHealth = 0;         // La vida actual del escudo
    private float currentHealth;                    // Vida actual de la nave
    private bool isDead = false;                    // Esto es para que si disparo con el doble disparo la nave no muera dos veces y spawnee mas power up de la cuenta guardo esta flag

    public event System.Action<float, float> OnHealthChanged; // (current, max) para UI u otros sistemas


    private void Awake(){
        // Inicializa la vida al maximo al instanciar
        currentHealth = maxHealth;
    }

    public void ChangeHealth(float value){
        // Modifica la vida (positiva cura, negativa hace dano). Si hay escudo activa y el dano es negativo primero se consume la escudo antes de afectar a la vida.

        if(activeBarrier&&value<0){
            currentbarrierHealth--;

            if(currentbarrierHealth<=0) {
                // Al agotarse la escudo, se pone el bool del animator para que se muestre en pantalla y se notifica al manager si es el player para que de el mensaje en pantalla
                activeBarrier = false;
                barrier.SetBool("BarrierUp", false);
                if(isPlayer){
                 if(GameOrchestrator.instance){
                    GameOrchestrator.instance.Barrier(false);
                 }
                }
            }
            return; // Si el escudo estaba activo ya sale de aqui para que no le haga dano al player
        }

        // Aplica el cambio a la vida
        currentHealth+=value;
        
        if (currentHealth>maxHealth){
            // Si la vida va a superar la maxima se establece la maxima y salgo de la funcion
            currentHealth = maxHealth;
            return;
        }

        // Notifica cambio de vida a otros scripts
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Chequea si se ha muerto el personaje
        if(currentHealth<=0&&!isDead){
            isDead=true;

            // Si es el jugador, le dice al manager que ponga el mensaje de muerte del player
            if(isPlayer){
                 if(GameOrchestrator.instance){
                    GameOrchestrator.instance.PlayerDead();
                 }
            }

            // Logica de drop si el Random.value (0..1), si es mayor que dropProb dropea un paquete
            bool randomPackGen = Random.value > dropProb;
            
            if (randomPackGen&&poweUpPrefab.Length>0)
            {
                // Elige un power-up aleatorio y lo instancia en la posicion del objeto
                int randomPowerUp = Random.Range(0, poweUpPrefab.Length);
                Instantiate(poweUpPrefab[randomPowerUp], transform.position, transform.rotation);
            }
            if(deathParticle!=null){
                // Separa el sistema de particulas para que no se destruya con el objeto y lo reproduce 
                deathParticle.transform.parent=null;
                deathParticle.Play();
            }
            // Destruye el objeto despues de todo
            Destroy(gameObject);
        }

        // Feedback visual de dano si esta el animator de la nave
        if(shipAnimator!=null&&value<0){
            shipAnimator.SetTrigger("Damage");
        }

    }

    public void AddBarrier(){
        // Activa el escudo si se puede activa el bool del animator de la barrera y notifica al manager para que salte el texto con el audio
        if(canHaveBarrier){
            activeBarrier=true;
            barrier.SetBool("BarrierUp", true);
            currentbarrierHealth = barriermaxHealth;
            if(isPlayer){
                 if(GameOrchestrator.instance){
                    GameOrchestrator.instance.Barrier(true);
                 }
            }
        }
    }

    public void AutoDestruction(){
        // Autodestruccion ponendo la vida a 0
        ChangeHealth(-maxHealth);
    }

}
