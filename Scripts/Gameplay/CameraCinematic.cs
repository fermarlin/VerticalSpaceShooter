using UnityEngine;

public class CameraCinematic : MonoBehaviour
{
    public void StartCinematic(){
        GameOrchestrator.instance.CinematicPause(true);
    }
    public void FinishCinematic(){
        GameOrchestrator.instance.CinematicPause(false);
    }
    
}
