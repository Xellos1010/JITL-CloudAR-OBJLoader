using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public void LoadScene(GameObject objectName)
    {
        SceneManager.LoadApplicationScene(objectName.name);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadApplicationScene(sceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadApplicationScene("Main");
    }
}
