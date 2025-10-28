using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MegaLaser : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private RectMask2D laserBar;
    [SerializeField]
    private Image laserBarImg;

    [Header("GameObject")]
    [SerializeField]
    private Animator megalaserAn;

    [SerializeField]
    private float megalaserDuration;

    private float megaLaserCharge = 0;
    private float maxHeight;
    private SpaceshipInput controls;

    void Awake()
    {
        controls = new SpaceshipInput(); 
        maxHeight = laserBarImg.rectTransform.sizeDelta.y;
    }


    void OnEnable()
    {
        controls.Spaceship.Enable();
        controls.Spaceship.MegaLaser.performed += OnMegaLaserPerformed;
    }

    void AddPoints(int points){
        megaLaserCharge+=points;
        if(megaLaserCharge>=1) megaLaserCharge=1;

        laserBar.padding = new Vector4(0,0,0,(1-megaLaserCharge)*maxHeight);
    }


    private void OnMegaLaserPerformed(InputAction.CallbackContext context)
    {
        if(!GameOrchestrator.instance.gamePaused){
            megalaserAn.SetBool("ActiveLaser", true);
        }
    }

}
