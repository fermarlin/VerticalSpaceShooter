using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 2;

    [SerializeField]
    [Range(0, 1)]
    private float dropProb = .5f;
    [SerializeField] 
    private ParticleSystem deathParticle;
    [SerializeField]
    private GameObject poweUpPrefab;

    private int currentHealth;

    private void Awake(){
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int value){
        currentHealth+=value;

        if (currentHealth>maxHealth){
            
            currentHealth = maxHealth;
            return;
        }

        if(currentHealth<=0){
            bool randomPackGen = Random.value > dropProb;

            if (randomPackGen&&poweUpPrefab!=null)
            {
                Instantiate(poweUpPrefab, transform.position, transform.rotation);
            }
            if(deathParticle!=null){
                deathParticle.transform.parent=null;
                deathParticle.Play();
            }
            Destroy(gameObject);
        }
    }

    public void AutoDestruction(){
        ChangeHealth(-maxHealth);
    }

}
