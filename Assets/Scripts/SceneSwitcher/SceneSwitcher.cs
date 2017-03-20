using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {
    public GameObject[] sceneObjects;
		
	void Start()
    {
        SceneManager.DisableAllScenes();
        SceneManager.InitializeDefaultScene();
    }

    public void ActivateScene(string name)
    {
        SceneManager.LoadIngameScene(name);
    }

    public void ActivateScene(GameObject sceneObject)
    {
        ActivateScene(sceneObject.name);
    }
}
