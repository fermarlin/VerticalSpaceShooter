using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipMovement : MonoBehaviour
{
    // Este script gestiona el movimiento del player

    [SerializeField] 
    private float speed = 5f;                 // Velocidad de desplazamiento lateral
    [SerializeField] 
    private float screenMargin = 0.05f;       // Margen del viewport para calcular los limites de movimiento

    private SpaceshipInput controls;          // Los controles del jugador, del newInputsystem
    private float moveX;                      // Valor del input horizontal
    private Camera cam;                       // Camara principal para conseguir los limites de la pantalla
    private float depth;                      // Profundidad de la nave respecto a la camara
    private float[] worldLimits = new float[2]; // Limites en X (izquierda/derecha)

    void Awake() {
       // Inicializa los controles
       controls = new SpaceshipInput(); 
    } 

    void Start(){
        // Calcula profundidad y limites de pantalla agregando ese margen indicado
        cam = Camera.main;
        depth = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);

        Vector3 leftWorld  = cam.ViewportToWorldPoint(new Vector3(screenMargin, 0f, depth));
        Vector3 rightWorld = cam.ViewportToWorldPoint(new Vector3(1f - screenMargin, 0f, depth));
        worldLimits[0] = leftWorld.x;
        worldLimits[1] = rightWorld.x;
    }

    void OnEnable()
    {
        // Habilita el input y se suscribe a las funciones de movimiento, tanto si esta pulsando como si ha soltado
        controls.Spaceship.Enable();
        controls.Spaceship.Movement.performed += OnMovementPerformed;
        controls.Spaceship.Movement.canceled += OnMovementCanceled;
    }

    void OnDisable(){
        // Desactiva los inputs si desactivamos este componente
        controls.Spaceship.Disable();
    } 

    void Update()
    {
        // No mover si el juego esta en pausa
        if(!GameOrchestrator.instance.gamePaused){
            // Mover en X segun el input y la velocidad
            Vector3 pos = transform.position;
            pos.x += moveX * speed * Time.deltaTime;

            // Limitar la posicion dentro de los bordes que hemos calculado en el start
            pos.x = Mathf.Clamp(pos.x, Mathf.Min(worldLimits[0], worldLimits[1]), Mathf.Max(worldLimits[0], worldLimits[1]));

            transform.position = pos;

            // Esto es una indicacion visual de la nave para que se vea que esta moviendose a un lado u otro
            float targetZRotation = moveX * -30f; 
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        // Lee el valor del eje y lo pasa a -1 o 1 de esta forma si estamos usando el mando aunque le demos poco a las palancas se movera del todo a un lado
        moveX = context.ReadValue<float>();
        moveX = Mathf.Sign(moveX);
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        // Al soltar el input, para el movimiento
        moveX = 0f;
    }
}
