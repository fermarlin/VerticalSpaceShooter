using UnityEngine;

public class CameraCinematic : MonoBehaviour
{
    // Esto es para gestionar las pausas de las cinematicas desde el animator
    public void StartCinematic(){
        // Activa la pausa de cinematica
        GameOrchestrator.instance.CinematicPause(true);
    }

    public void FinishCinematic(){
        // Desactiva la pausa de cinematica
        GameOrchestrator.instance.CinematicPause(false);
    }
    
}
