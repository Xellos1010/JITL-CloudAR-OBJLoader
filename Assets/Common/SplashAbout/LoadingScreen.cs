/*===============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour 
{
    #region INSTANCE_VARIABLES
    public static LoadingScreen instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<LoadingScreen>();
            return _instance;
        }
    }
    private static LoadingScreen _instance;
    #endregion // INSTANCE_VARIABLES

    #region PRIVATE_MEMBER_VARIABLES    
    public bool mChangeLevel = false;
    private RawImage mUISpinner;
    private Canvas mCanvas;
    private string levelLoaded = "";
    #endregion // PRIVATE_MEMBER_VARIABLES

    #region PUBLIC_MEMBER_VARIABLES
    public string levelToLoad = "";
    public float loadingIconSpinSpeed = 120f;
    #endregion //PUBLIC_MEMBER_VARIABLES

    #region MONOBEHAVIOUR_METHODS
    void Start () 
    {
        mUISpinner = FindSpinnerImage();
        mCanvas = FindCanvas();
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        //LoadLevelASync("MainMenu");
        LoadLevelASyncAdditive("Main");
        //SetLevelToLoad("MainMenu");
    }    
    
    void Update () 
    {
        if (mCanvas.gameObject.activeSelf)
            mUISpinner.rectTransform.Rotate(Vector3.forward, loadingIconSpinSpeed * Time.deltaTime);        
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS
    private IEnumerator LoadNextSceneAsync()
    {
        yield return new WaitForEndOfFrame();
        yield return UnloadCurrentScene();
        yield return new WaitForEndOfFrame();
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        //yield return Application.LoadLevelAsync(levelToLoad);
        yield return Application.LoadLevelAdditiveAsync(levelToLoad);
#else // UNITY_5_3 or above
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelToLoad,UnityEngine.SceneManagement.LoadSceneMode.Additive);
#endif
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(levelToLoad).IsValid())
        {
            Debug.Log("Scene Management loaded " + levelToLoad + " and it is valid");            
            SetCurrentLoadedScene();
        }    
        else
        {
            Debug.Log("Scene Management tried to load level " + levelToLoad + ". It's not valid");          
        }
        ResetLevelChange();
        ToggleLoadingCanvas(false);
    }

    private void ResetLevelChange()
    {
        mChangeLevel = false;
        GameObject[] SceneObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Main").GetRootGameObjects();
        foreach (GameObject rootObject in SceneObjects)
        {
            if (rootObject.name.Contains("SceneManager"))
            {
                rootObject.GetComponent<MenuBarSceneLoader>().menuBarLoading = false;
            }
        }
    }

    private void ToggleLoadingCanvas(bool OnOff)
    {
        mCanvas.gameObject.SetActive(OnOff);
    }

    private RawImage FindSpinnerImage()
    {
        RawImage[] images = FindObjectsOfType<RawImage>();
        foreach (var img in images)
        {
            if (img.name.Contains("Spinner"))
            {
                return img;
            }
        }
        return null;
    }

    private Canvas FindCanvas()
    {
        Canvas[] canvas = FindObjectsOfType<Canvas>();
        foreach (var child in canvas)
        {
            if (child.name.Contains("Loading"))
            {
                return child;
            }
        }
        return null;
    }

    private void LoadLevelASyncAdditive(string levelToLoad)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelToLoad,UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    private void SetCurrentLoadedScene()
    {
        levelLoaded = levelToLoad;
        levelToLoad = "";
    }

    private IEnumerator UnloadCurrentScene()
    {
        if (levelLoaded != "")
            yield return UnityEngine.SceneManagement.SceneManager.UnloadScene(levelLoaded);
        else
            yield return null;
        levelLoaded = "";
    }
    #endregion // PRIVATE_METHODS

    #region PUBLIC_METHODS
    public void SetLevelToLoad(string loadLevel)
    {
        if (!mChangeLevel)
        {
            ToggleLoadingCanvas(true);
            levelToLoad = loadLevel;
            mChangeLevel = true;
            StartCoroutine(LoadNextSceneAsync());
        }
    }

    public void SetLevelToLoad(string loadLevel,float waituntil)
    {
        if (!mChangeLevel)
        {
            levelToLoad = loadLevel;
            ToggleLoadingCanvas(true);
            StartCoroutine(WaitToLoadLevel(waituntil));        
        }
    }

    IEnumerator WaitToLoadLevel(float amount)
    {
        yield return new WaitForSeconds(amount);
        mChangeLevel = true;
        StartCoroutine(LoadNextSceneAsync());
    }
    #endregion //PUBLIC_METHODS
}
