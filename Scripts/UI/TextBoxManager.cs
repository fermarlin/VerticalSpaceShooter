using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TextBoxManager : MonoBehaviour
{
    // Gestiona el cuadro de texto y el audio del texto

    public static TextBoxManager instance;

    [SerializeField]
    private TextMeshProUGUI txtBox;     // Referencia al texto que muestra el mensaje

    [SerializeField]
    private AudioClip DialUp;           // Sonido previo antes del mensaje

    private Animator textBoxAnimator;   // Animator del cuadro de texto, para abrir o cerrar el panel

    private void Awake(){
        // Inicializa para poder llamar donde sea y pillo el Animator del objeto
        if (instance == null)
        {
            instance = this;
        }
        textBoxAnimator=GetComponent<Animator>();
    }

    public IEnumerator ChangeTextCoroutine(string text, AudioClip audioText)
    {
        // Cancela cualquier audio en curso para que el mensaje suene limpio
        SoundManager.instance.AudioCancel();

        // Pone el texto y lo muestra
        txtBox.text = text;
        textBoxAnimator.SetBool("MssgActive", true);

        // Reproduce el DialUp si esta y espera un poco
        if (DialUp != null)
        {
            SoundManager.instance.PlayAudioClipDefaultPitch(DialUp);
            yield return new WaitForSeconds(0.5f);
        }

        // Reproduce el audio del mensaje y espera a que termine
        if (audioText != null)
        {
            SoundManager.instance.PlayAudioClipDefaultPitch(audioText);
            yield return new WaitForSeconds(audioText.length);
        }

        // Oculta el cuadro de texto
        textBoxAnimator.SetBool("MssgActive", false);
    }
}
