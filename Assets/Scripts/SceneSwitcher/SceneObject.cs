using UnityEngine;
using System.Collections;

/// <summary>
/// Used to define the parent scene GameObject.
/// Holds all objects to activate in-game scene for UI or 3d Objects.
/// </summary>
public class SceneObject : MonoBehaviour
{
    /// <summary>
    /// Toggle on to set this scene as the scene that remains active on Start
    /// </summary>
    public bool setActiveOnAwake;
	// Use this for initialization
	void Awake()
    {
        //Registers the scene with the Scene Manager
        SceneManager.RegisterIngameScene(this);

        if (!setActiveOnAwake)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
	
}
