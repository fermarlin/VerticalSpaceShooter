using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipMovement : MonoBehaviour
{
    [SerializeField] 
    private float speed = 5f;
    [SerializeField] 
    private float screenMargin = 0.05f;

    private SpaceshipInput controls;
    private float moveX;
    private Camera cam;
    private float depth;
    private float[] worldLimits = new float[2];

    void Awake() {
       controls = new SpaceshipInput(); 
    } 

    void Start(){
        cam = Camera.main;
        depth = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);

        Vector3 leftWorld  = cam.ViewportToWorldPoint(new Vector3(screenMargin, 0f, depth));
        Vector3 rightWorld = cam.ViewportToWorldPoint(new Vector3(1f - screenMargin, 0f, depth));
        worldLimits[0] = leftWorld.x;
        worldLimits[1] = rightWorld.x;
    }

    void OnEnable()
    {
        controls.Spaceship.Enable();
        controls.Spaceship.Movement.performed += OnMovementPerformed;
        controls.Spaceship.Movement.canceled += OnMovementCanceled;
    }

    void OnDisable(){
        controls.Spaceship.Disable();
    } 

    void Update()
    {
        if(!GameOrchestrator.instance.gamePaused){
            Vector3 pos = transform.position;
            pos.x += moveX * speed * Time.deltaTime;

            pos.x = Mathf.Clamp(pos.x, Mathf.Min(worldLimits[0], worldLimits[1]), Mathf.Max(worldLimits[0], worldLimits[1]));

            transform.position = pos;

            float targetZRotation = moveX * -30f; 
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }


    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveX = context.ReadValue<float>();
        moveX = Mathf.Sign(moveX);
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveX = 0f;
    }
    



}
