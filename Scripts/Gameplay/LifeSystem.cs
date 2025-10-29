using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField]
    private int maxHealth = 2;
    [SerializeField] 
    private ParticleSystem deathParticle;
    [SerializeField] 
    private Animator shipAnimator;

    [Header("Power UP Parameters")]
    [SerializeField]
    [Range(0, 1)]
    private float dropProb = .5f;
    [SerializeField]
    private GameObject[] poweUpPrefab;

    [Header("Barrier Parametres")]
    [SerializeField]
    private bool canHaveBarrier = false;
    [SerializeField]
    private int barriermaxHealth = 2;
    [SerializeField]
    private Animator barrier;
    
    [Header("Game Orchestrator Parametres")]
    [SerializeField]
    private bool sendInfoToGO = false;

    private bool activeBarrier = false;
    private float currentbarrierHealth = 0;
    private float currentHealth;
    private bool isDead = false;
    private void Awake(){
        currentHealth = maxHealth;
    }

    public void ChangeHealth(float value){

        if(activeBarrier&&value<0){
            currentbarrierHealth--;

            if(currentbarrierHealth<=0) {
                activeBarrier = false;
                barrier.SetBool("BarrierUp", false);
                if(sendInfoToGO){
                 if(GameOrchestrator.instance){
                    GameOrchestrator.instance.Barrier(false);
                 }
                }
            }
            return;
        }

        currentHealth+=value;

        if (currentHealth>maxHealth){
            
            currentHealth = maxHealth;
            return;
        }

        if(currentHealth<=0&&!isDead){
            isDead=true;
            bool randomPackGen = Random.value > dropProb;
            
            if (randomPackGen&&poweUpPrefab.Length>0)
            {
                int randomPowerUp = Random.Range(0, poweUpPrefab.Length);
                Instantiate(poweUpPrefab[randomPowerUp], transform.position, transform.rotation);
            }
            if(deathParticle!=null){
                deathParticle.transform.parent=null;
                deathParticle.Play();
            }
            Destroy(gameObject);
        }

        if(shipAnimator!=null&&value<0){
            shipAnimator.SetTrigger("Damage");
        }
    }

    public void AddBarrier(){
        if(canHaveBarrier){
            activeBarrier=true;
            barrier.SetBool("BarrierUp", true);
            currentbarrierHealth = barriermaxHealth;
            if(sendInfoToGO){
                 if(GameOrchestrator.instance){
                    GameOrchestrator.instance.Barrier(true);
                 }
            }
        }
    }

    public void AutoDestruction(){
        ChangeHealth(-maxHealth);
    }

}
