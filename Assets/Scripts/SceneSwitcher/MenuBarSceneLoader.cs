using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuBarSceneLoader : SceneLoadPass
{
    public static MenuBarSceneLoader instance
    {
        get
        {
            if (_instance = null)
                _instance = GameObject.FindGameObjectWithTag("MainMenuBar").GetComponent<MenuBarSceneLoader>();
            return _instance;
        }
    }
    private static MenuBarSceneLoader _instance;
    public bool menuBarLoading = false;
    public RectTransform ArrowMovementBar; //currently manuelly setting the arrow to move
    public Transform buttonHolderMaster;
    public float arrowTransitionTime;
    public float pendingColorBlinkDuration;
    public Color activeButtonColor;
    public Color inactiveButtonColor;
    public Color pendingButtonColor;
    public Color errorButtonColor;
    ButtonStateManager currentPendingButton;
    public iTween.EaseType easeType = iTween.EaseType.easeOutExpo;
    //public string currentlyActiveMainScene = "MainMenu";
    public GameObject currentActiveButton; //Currently setting starting active button to main menu

    void Start()
    {          
        LoadSceneApplication("MainMenu");
    }

    public void LoadMainScene(GameObject Button) // Label the button name the same as the scene name
    {
        Debug.Log("Loading Main Scene " + Button.name);
        if (!menuBarLoading)
        {
            menuBarLoading = true;
            SetActiveButton(Button);
            LoadSceneApplicationWithWait(Button.name, arrowTransitionTime); // Load the loading screen but wait for the arrow to finish transitioning to have a smoother experience
        }
    }

    bool CheckLoadingFinished()
    {
        GameObject[] SceneObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Loading").GetRootGameObjects();
        foreach (GameObject rootObject in SceneObjects)
        {
            if (rootObject.name.Contains("LoadingManager"))
            {
                Debug.Log("" + rootObject.GetComponent<LoadingScreen>().mChangeLevel);
                return rootObject.GetComponent<LoadingScreen>().mChangeLevel;
            }
        }
        throw new System.Exception("There was an issue accessing LoadingManager. Ensure it's added to your scene builds and loaded before the MainBar Scene \"Main\"");
    }

    public void LoadMainScene(int buttonNumber) //The button Child Scene to Load
    {
        GameObject Button = buttonHolderMaster.GetChild(buttonNumber).gameObject;
        LoadMainScene(Button);        
    }

    /// <summary>
    /// Called from VWSCloudConnecter to blink a color while image target is uploading
    /// </summary>
    /// <param name="button">name of the UploadImageVuphoria or any button</param>
    public void BlinkButtonPendingColor(string button = "UploadImageVuphoria")
    {
        Debug.Log("Checking for currentPendingButton");
        if (currentPendingButton == null)
        {
            Debug.Log("currentPendingButton == null");
            foreach (Transform child in buttonHolderMaster)
            {
                Debug.Log("Checking for child with name " + button);
                if (child.gameObject.name == button)
                {
                    Debug.Log("found transform child with name " + button);
                    currentPendingButton = child.GetComponent<ButtonStateManager>();
                    currentPendingButton.ActivatePendingState(pendingButtonColor, inactiveButtonColor, pendingColorBlinkDuration);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("There is a current pending button " + currentPendingButton.gameObject.name);
            currentPendingButton.ActivatePendingState(pendingButtonColor, inactiveButtonColor, pendingColorBlinkDuration);
        }
    }

    /// <summary>
    /// Called from VWSCloudConnecter to blink a color while image target is uploading
    /// </summary>
    /// <param name="button">name of the UploadImageVuphoria or any button</param>
    public void ErrorButtonPending(string button = "UploadImageVuphoria")
    {
        if (currentPendingButton == null)
        {
            foreach (Transform child in buttonHolderMaster)
                if (child.gameObject.name == button)
                {
                    currentPendingButton = child.GetComponent<ButtonStateManager>();
                    currentPendingButton.ChangeColor(errorButtonColor);
                    break;
                }
        }
        else
        {
            currentPendingButton.ChangeColor(errorButtonColor);
        }
    }

    public void SwitchPendingToActive()
    {
        if (currentPendingButton)
            currentPendingButton.DeactivatePendingState();
        currentPendingButton.ChangeColor(activeButtonColor);
        StartCoroutine(TurnCheckTurnOffPendingButton());
    }

    IEnumerator TurnCheckTurnOffPendingButton()
    {
        yield return new WaitForSeconds(5);
        if (currentPendingButton != currentActiveButton)
            ToggleButton(currentPendingButton.gameObject, false);
        currentPendingButton = null;
    }

    public void FadeColor()
    {

    }

    /*public void UnloadActiveScene()
    {
        SceneManager.UnloadApplicationScene(currentlyActiveMainScene);
        //currentlyActiveMainScene = "";
    }*/

    public void LoadMainScene(string childName) //The button Child Scene to Load
    {
        GameObject Button;
        for(int i = 0; i < buttonHolderMaster.transform.childCount; i++)
        {
            if (buttonHolderMaster.GetChild(i).gameObject.name == childName)
            {
                Button = buttonHolderMaster.GetChild(i).gameObject;
                LoadMainScene(Button);
                break;
            }
        }        
    }

    private void LoadSceneApplication(string sceneName)
    {
        PassToLoadingScreen(sceneName);
    }

    private void LoadSceneApplicationWithWait(string sceneName,float waitTime)
    {
        PassToLoadingScreen(sceneName, waitTime);
    }

    public void PassToLoadingScreen(string sceneToLoad)
    {
        GameObject[] SceneObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Loading").GetRootGameObjects();
        foreach (GameObject rootObject in SceneObjects)
        {
            if (rootObject.name.Contains("LoadingManager"))
                rootObject.GetComponent<LoadingScreen>().SetLevelToLoad(sceneToLoad);
        }
    }

    public void PassToLoadingScreen(string sceneToLoad,float amountToWait)
    {
        GameObject[] SceneObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Loading").GetRootGameObjects();
        foreach (GameObject rootObject in SceneObjects)
        {
            if (rootObject.name.Contains("LoadingManager"))
                rootObject.GetComponent<LoadingScreen>().SetLevelToLoad(sceneToLoad, amountToWait);
        }
    }

    private void SetActiveButton(GameObject buttonToActivate)
    {
        Debug.Log("Setting Active Button ");
        TransitionArrowToButton(buttonToActivate);
        ToggleAllButtons(false);
        currentActiveButton = buttonToActivate;
        ToggleButton(currentActiveButton, true);
    }

    void ToggleAllButtons(bool onOff)
    {
        foreach(Transform child in buttonHolderMaster)
        {
            if (!child.GetComponent<ButtonStateManager>().pendingActive)
            {
                if (onOff)
                    currentActiveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = activeButtonColor;
                else
                    currentActiveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = inactiveButtonColor;
            }
        }
    }

    private void ToggleButton(GameObject currentActiveButton, bool state)
    {
        if (!currentActiveButton.GetComponent<ButtonStateManager>().pendingActive)
        {
            if (state)
                currentActiveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = activeButtonColor;
            else
                currentActiveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = inactiveButtonColor;
        }
    }

    private void TransitionArrowToButton(GameObject targetButton)
    {
        float placeOnScreen = targetButton.GetComponent<RectTransform>().anchoredPosition.x;
 
        Hashtable ValueToArguments = iTween.Hash(
            "from", ArrowMovementBar.offsetMin.x,
            "to", placeOnScreen,
            "time", arrowTransitionTime,
            "easetype", easeType,
            "onupdate", "UpdateArrowValue",
            "onupdatetarget", gameObject
            );
        iTween.ValueTo(gameObject, ValueToArguments);
        Debug.Log("itween Done");
    }

    public void UpdateArrowValue(float newValue)
    {
        ArrowMovementBar.anchoredPosition = new Vector2(newValue, ArrowMovementBar.anchoredPosition.y);
    }

    private int OrderInParent(Transform target)
    {
        for (int i = 0; i < target.parent.childCount; i++)
            if (target.parent.GetChild(i).name == target.name)
                return i;
        return 0;
    }
}
