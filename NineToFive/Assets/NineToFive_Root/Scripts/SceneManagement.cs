using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void NextScene(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
