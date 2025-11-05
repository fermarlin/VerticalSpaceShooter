using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadManager : MonoBehaviour
{
    // Aqui gestiono el cambio de escena, ademas hago un fundido con una imagen en pantalla.

    public static LoadManager instance;
    [SerializeField] private Image whiteForeGround;  // Imagen en la UI que uso para hacer el fundido a blanco
    [SerializeField] private float fadeSpeed = 2f;   // Velocidad del fundido

    private void Awake()
    {
        // Pongo igual que el GameOrchestrator para que pueda acceder al mismo desde cualquier punto sin tener que agregarlo en el inspector y le digo que no lo destruya cuando cambia de escena para que haga bien el fade out
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadScene(int index)
    {
        // Arranca el proceso de cambio de escena
        StopAllCoroutines();
        StartCoroutine(LoadNextScene(index));
    }

    private IEnumerator LoadNextScene(int index)
    {
        // Fade-out a blanco para ocultar la escena
        yield return StartCoroutine(FadeAlpha(1f));

        // Carga la nueva escena de forma asincrona sin activarla aún
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = false;

        // Espera a que la escena ya este practicamente cargada
        while (asyncLoad.progress < 0.9f)
            yield return null;

        // Activa la escena y hace fade out
        asyncLoad.allowSceneActivation = true;
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(FadeAlpha(0f));
    }

    private IEnumerator FadeAlpha(float targetAlpha)
    {
        // Interpola la alpha de la imagen blanca
        if (whiteForeGround == null) yield break;

        Color color = whiteForeGround.color;
        float startAlpha = color.a;
        float time = 0f;

        while (!Mathf.Approximately(color.a, targetAlpha))
        {
            time += Time.deltaTime * fadeSpeed;           // Esto es para que poco a poco vaya pasando de un alpha a otro
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time);
            whiteForeGround.color = color;
            yield return null;
        }
    }
}
