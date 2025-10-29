using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;
    [SerializeField] private Image whiteForeGround;
    [SerializeField] private float fadeSpeed = 2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadScene(int index)
    {
        StopAllCoroutines();
        StartCoroutine(LoadNextScene(index));
    }

    private IEnumerator LoadNextScene(int index)
    {
        // Fade-out
        yield return StartCoroutine(FadeAlpha(1f));

        // Cargar escena de forma asíncrona
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = false;

        // Esperar hasta que esté casi cargada
        while (asyncLoad.progress < 0.9f)
            yield return null;

        // Activar escena y hacer fade-in
        asyncLoad.allowSceneActivation = true;
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(FadeAlpha(0f));
    }

    private IEnumerator FadeAlpha(float targetAlpha)
    {
        if (whiteForeGround == null) yield break;

        Color color = whiteForeGround.color;
        float startAlpha = color.a;
        float t = 0f;

        while (!Mathf.Approximately(color.a, targetAlpha))
        {
            t += Time.deltaTime * fadeSpeed;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            whiteForeGround.color = color;
            yield return null;
        }
    }
}
