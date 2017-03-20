using UnityEngine;
using System.Collections;

public class SceneLoadPass : MonoBehaviour {

	public void PassToLoader(string sceneToLoad)
    {
        GameObject[] SceneObjects =  UnityEngine.SceneManagement.SceneManager.GetSceneByName("Main").GetRootGameObjects();
        foreach (GameObject rootObject in SceneObjects)
        {
            if (rootObject.name.Contains("SceneManager"))
                rootObject.GetComponent<MenuBarSceneLoader>().LoadMainScene(sceneToLoad);
        }
    }    
}
