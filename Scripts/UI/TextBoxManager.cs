using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TextBoxManager : MonoBehaviour
{
    public static TextBoxManager instance;

    [SerializeField]
    private TextMeshProUGUI txtBox;

    [SerializeField]
    private AudioClip DialUp;

    private Animator textBoxAnimator;

    private void Awake(){
        if (instance == null)
        {
            instance = this;
        }
        textBoxAnimator=GetComponent<Animator>();
    }

    public IEnumerator ChangeTextCoroutine(string text, AudioClip audioText)
    {
        SoundManager.instance.AudioCancel();
        txtBox.text = text;
        textBoxAnimator.SetBool("MssgActive", true);

        if (DialUp != null)
        {
            SoundManager.instance.PlayAudioClipDefaultPitch(DialUp);
            yield return new WaitForSeconds(0.5f);
        }

        if (audioText != null)
        {
            SoundManager.instance.PlayAudioClipDefaultPitch(audioText);
            yield return new WaitForSeconds(audioText.length);
        }
        textBoxAnimator.SetBool("MssgActive", false);
    }
}
