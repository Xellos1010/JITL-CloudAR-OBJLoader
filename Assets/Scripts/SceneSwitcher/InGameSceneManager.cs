using UnityEngine;
using System.Collections.Generic;

public class InGameSceneManager : MonoBehaviour
{

    public static InGameSceneManager instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<InGameSceneManager>();
            return _instance;
        }
    }
    private static InGameSceneManager _instance;
	// Use this for initialization
	void Awake()
    {
        //ToggleAllScenes(true);
        //FocusAwakeScene();
        try
        {
            SceneManager.AlphaManagerCanvasGroup = GetComponent<CanvasGroup>();
        }
        catch
        {
            Debug.Log("SceneManager doesn't contain Canvas Group");
        }
	}

    void OnEnable()
    {
        _instance = this;
    }

    void OnDisable()
    {
        _instance = null;
    }

    public void LoadIngameScene(GameObject sceneName)
    {
        SceneManager.LoadIngameScene(sceneName.name);
    }

    public void LoadIngameScene(string sceneName)
    {
        SceneManager.LoadIngameScene(sceneName);
    }

    public void ToggleAllScenes(bool onOff)
    {
        foreach(Transform childScene in transform)
        {
            if (childScene.GetComponent<SceneObject>() != null)
                childScene.gameObject.SetActive(onOff);
        }
    }

    public void FocusAwakeScene()
    {
        foreach (Transform childScene in transform)
        {
            if (childScene.GetComponent<SceneObject>() != null)
                childScene.gameObject.SetActive(childScene.GetComponent<SceneObject>().setActiveOnAwake);
        }
    }

    public void FocusSceneActive(SceneObject scene)
    {
        ToggleAllScenes(false);
        scene.gameObject.SetActive(true);
    }
}
