using UnityEngine;
using System.Collections;

public class GameOrchestrator : MonoBehaviour
{
    public static GameOrchestrator instance;
    public bool gamePaused = false;

    private bool cinematic = false;

    private void Awake(){

        if (instance == null)
        {
            instance = this;
        }
    }

    public void CinematicPause(bool value){
        cinematic=value;
        PauseGame(value);
    }

    public void PauseGame(bool value){
        if(cinematic){
            gamePaused = true;
        }else 
        {
            gamePaused = value;
        }
    }


    public IEnumerator PausableWait(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            if (!gamePaused)
                t += Time.deltaTime;

            yield return null;
        }
    }
}
