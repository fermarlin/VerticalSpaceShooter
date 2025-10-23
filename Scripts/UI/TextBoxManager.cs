using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TextBoxManager : MonoBehaviour
{
    [System.Serializable]
    public struct TextPremade
    {
        public string textPremade;
        public AudioClip audioC;
    }
    [SerializeField]
    private TextMeshProUGUI txtBox;
    [SerializeField]
    private TextPremade[] textsPremades;
    [SerializeField]
    private AudioClip DialUp;

    private Animator textBoxAnimator;

    private void Awake(){
        textBoxAnimator=GetComponent<Animator>();
    }

    private void Start(){
        int introMssg = Random.Range(0, textsPremades.Length-1);
        StartCoroutine(ChangeTextCoroutine(introMssg));
    }

    private IEnumerator ChangeTextCoroutine(int index)
    {
        txtBox.text = textsPremades[index].textPremade;
        textBoxAnimator.SetBool("MssgActive", true);

        if (DialUp != null)
        {
            SoundManager.instance.PlayAudioClipDefaultPitch(DialUp);
            yield return new WaitForSeconds(0.5f);
        }

        if (textsPremades[index].audioC != null)
        {
            SoundManager.instance.PlayAudioClipDefaultPitch(textsPremades[index].audioC);
            yield return new WaitForSeconds(textsPremades[index].audioC.length);
        }
        textBoxAnimator.SetBool("MssgActive", false);
    }
}
