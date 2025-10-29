using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class MegaLaser : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectMask2D laserBar;
    [SerializeField] private Image laserBarImg;

    [Header("GameObject")]
    [SerializeField] private Animator megalaserAn;

    [Header("Config")]  
    [SerializeField] private float megalaserDuration = 3f;  
    [SerializeField] private float megalaserCooldown = 20f;  
    [SerializeField] private int megalaserDPS = -1;
    [SerializeField] private string objetiveTag = null;

    private bool megaLaserActive = false;
    private bool canShoot = false;
    private float megaLaserCharge = 0f;
    private float maxHeight;
    private SpaceshipInput controls;
    private List<LifeSystem> enemiesLifeSystems = new List<LifeSystem>();

    void Awake()
    {
        controls = new SpaceshipInput();
        maxHeight = laserBarImg.rectTransform.sizeDelta.y;
    }

    void Start()
    {
        StartCoroutine(ChargeLaser());
    }

    void OnEnable()
    {
        controls.Spaceship.Enable();
        controls.Spaceship.MegaLaser.performed += OnMegaLaserPerformed;
    }

    void OnDisable()
    {
        controls.Spaceship.Disable();
    }

    void Update(){
        if(megaLaserActive&&enemiesLifeSystems.Count>0){
            for (int i = enemiesLifeSystems.Count - 1; i >= 0; i--)
            {
                if (enemiesLifeSystems[i] == null)
                {
                    enemiesLifeSystems.RemoveAt(i);
                    continue;
                }

                enemiesLifeSystems[i].ChangeHealth((megalaserDPS * Time.deltaTime));
            }
        }
    }

    private void OnMegaLaserPerformed(InputAction.CallbackContext context)
    {
        if (!GameOrchestrator.instance.gamePaused && !megaLaserActive && canShoot)
        {
            megalaserAn.SetBool("ActiveLaser", true);
            megaLaserActive = true;
            StartCoroutine(ActiveLaser());
        }
    }

    private void MegaLaserUIUpdate()
    {
        laserBar.padding = new Vector4(0, 0, 0, (1 - megaLaserCharge / megalaserCooldown) * maxHeight);
    }

    private IEnumerator ActiveLaser()
    {
        float timer = 0f;
        float initialCharge = megaLaserCharge;
        while (timer < megalaserDuration)
        {
            while (GameOrchestrator.instance.gamePaused) yield return null;

            timer += Time.deltaTime;
            megaLaserCharge = Mathf.Lerp(initialCharge, 0f, timer / megalaserDuration);
            MegaLaserUIUpdate();

            yield return null;
        }

        megaLaserCharge = 0f;
        MegaLaserUIUpdate();

        megalaserAn.SetBool("ActiveLaser", false);
        megaLaserActive = false;
        canShoot = false;

        StartCoroutine(ChargeLaser());
    }

    private IEnumerator ChargeLaser()
    {
        float timer = 0f;
        while (!canShoot)
        {
            while (GameOrchestrator.instance.gamePaused) yield return null;

            timer += Time.deltaTime;
            megaLaserCharge = Mathf.Lerp(0f, megalaserCooldown, timer / megalaserCooldown);
            MegaLaserUIUpdate();

            if (megaLaserCharge >= megalaserCooldown)
            {
                megaLaserCharge = megalaserCooldown;
                canShoot = true;
            }

            yield return null;
        }
        GameOrchestrator.instance.MegaLaserRecharged();
    }

    public bool UsingMLaser(){
        return megaLaserActive;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(objetiveTag))
        {
            enemiesLifeSystems.Add(other.GetComponent<LifeSystem>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(objetiveTag))
        {
            enemiesLifeSystems.Remove(other.GetComponent<LifeSystem>());
        }
    }
}
