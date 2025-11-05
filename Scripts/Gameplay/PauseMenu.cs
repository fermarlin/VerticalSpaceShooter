using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Este script es para poder llamar al LoadManager independientemente del que haya en escena (por si saltamos de una escena u otra y ponerlo por el inspector falla ya que lo sobrescribo)

    public void ResetScene(){
        // Recarga la escena actual
        int index = SceneManager.GetActiveScene().buildIndex;
        LoadManager.instance.LoadScene(index);
    }

    public void ReturnToMenu(){
        // Vuelve al menu principal
        LoadManager.instance.LoadScene(0);
    }

    public void LoadScene(int index){
        // Carga una escena concreta
        LoadManager.instance.LoadScene(index);
    }

    public void QuitApp(){
        // Cierra la aplicacion, esto es por si se hiciera una build poder salir sin ALT F4
        Application.Quit();
    }
}
