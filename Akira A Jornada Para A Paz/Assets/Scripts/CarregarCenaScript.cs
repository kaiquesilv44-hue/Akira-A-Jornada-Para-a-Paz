using UnityEngine;

public class CarregarCenaScript : MonoBehaviour
{
   public string sceneName;

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
