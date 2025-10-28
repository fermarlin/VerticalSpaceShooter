using UnityEngine;
using System.Collections;

public class GameOrchestrator : MonoBehaviour
{
    public static GameOrchestrator instance;
    public bool gamePaused = false;

    private bool cinematic = false;
    private bool hasGotAnyBarrier = false;

    [System.Serializable]
    public struct TextPremade
    {
        public string textPremade;
        public AudioClip audioC;
    }

    [SerializeField]
    private TextPremade[] initTexts;
    [SerializeField]
    private TextPremade[] shieldUpText;
    [SerializeField]
    private TextPremade[] shieldDownText;
    Coroutine textCoroutine;

    private void Awake(){

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start(){
        int introMssg = Random.Range(0, initTexts.Length);
        StartCoroutine(TextBoxManager.instance.ChangeTextCoroutine(initTexts[introMssg].textPremade, initTexts[introMssg].audioC));
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

    public void Barrier(bool up){
        if(!hasGotAnyBarrier && up){
            if(textCoroutine!=null) {
                StopCoroutine(textCoroutine);
            }
            int shieldUpMssg = Random.Range(0, shieldUpText.Length);
            textCoroutine = StartCoroutine(TextBoxManager.instance.ChangeTextCoroutine(shieldUpText[shieldUpMssg].textPremade, shieldUpText[shieldUpMssg].audioC));
            hasGotAnyBarrier=true;
        }
        if(!up){
            if(textCoroutine!=null) {
                StopCoroutine(textCoroutine);
            }
            int shieldDownMssg = Random.Range(0, shieldDownText.Length);
            textCoroutine = StartCoroutine(TextBoxManager.instance.ChangeTextCoroutine(shieldDownText[shieldDownMssg].textPremade, shieldDownText[shieldDownMssg].audioC));
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
