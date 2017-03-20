using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckUploadProgress : MonoBehaviour {

    public string targetID;
    MenuBarSceneLoader sceneLoaderToCall;
    public string mainURL = "http://www.augmentourworld.com/Vuphoria/php/main.php";

    void Awake()
    {

    }

    void InitializeCallbacks()
    {
        //FacebookShare.initialized += facebookInitialized;
    }

    public CheckUploadProgress(string targetIDToCheck, MenuBarSceneLoader loader)
    {
        targetID = targetIDToCheck;
        sceneLoaderToCall = loader;
    }

    public void GetCheckTargetIDInfo()
    {
        Debug.Log("Starting Coroutine");
        StartCoroutine(GetTargetIDInfoInitialize());
    }

    public void GetCheckTargetIDInfo(string targetIDToCheck, MenuBarSceneLoader loader, string url)
    {
        mainURL = url;
        targetID = targetIDToCheck;
        sceneLoaderToCall = loader;
        GetCheckTargetIDInfo();
    }

    IEnumerator GetTargetIDInfoInitialize()
    {
        yield return GetAllTargetIDs();
        StartCoroutine(GetTargetIDInfo());
    }
    IEnumerator GetTargetIDInfo()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("Getting TargetID Info");
        WWWForm postForm = new WWWForm();
        postForm.AddField("select", phpModeSelet.GetTargetInfo.ToString());
        postForm.AddField("targetID", targetID);
        //string URLtoUse = "http://www.augmentourworld.com/Vuphoria/php/main.php";
        WWW getTargetIDinfo = new WWW(mainURL, postForm);
        yield return getTargetIDinfo;
        if (getTargetIDinfo.error == null)
        {
            Debug.Log(getTargetIDinfo.text);
            JSONObject jsonTargetSummary = new JSONObject(getTargetIDinfo.text);
            try {
                if (jsonTargetSummary.GetField("status").str == "processing")
                {
                    Debug.Log("Still processing run the script again");
                    StartCoroutine(GetTargetIDInfo());
                }
                else
                {
                    sceneLoaderToCall.SwitchPendingToActive();
                    Destroy(this.gameObject);
                }
            }
            catch
            {
                Debug.Log("ImproperJSON Formatting Trying again");
                Debug.Log("targetID = " + targetID);
                Debug.Log("mainURL = " + mainURL);
                Debug.Log("getTargetIDinfo.url = " + getTargetIDinfo.url);
                StartCoroutine(GetTargetIDInfo());
            }
        }
        else
            Debug.Log("Error during Getting Target ID's: " + getTargetIDinfo.error + "getTargetIDinfo.url = " + getTargetIDinfo.url);
    }

    IEnumerator GetAllTargetIDs()
    {
        Debug.Log("Getting all Target ID's");
        WWWForm postForm = new WWWForm();
        postForm.AddField("select", phpModeSelet.GetAllTargets.ToString());
        WWW getTargetIDs = new WWW(mainURL, postForm);
        yield return getTargetIDs;
        if (getTargetIDs.error == null)
        {
            List<string> strintargetIDs = ParseReturnString(getTargetIDs.text);
            string targetIDRange = targetID.Substring(4, 8 - 4 + 1);
            foreach (string ID in strintargetIDs)
            {
                if (ID.Contains(targetIDRange))
                {
                    targetID = ID;
                    Debug.Log("Target ID was found and assigned");
                    break;
                }
            }
            Debug.Log("targetID " + targetID);
            Debug.Log("TargetID = " + strintargetIDs[strintargetIDs.Count - 1]);
            //targetID = strintargetIDs[strintargetIDs.Count - 1];
            Debug.Log("Getting Target ID's done :" + getTargetIDs.text);
        }
        else
            Debug.Log("Error during Getting Target ID's: " + getTargetIDs.error);
    }

    List<string> ParseReturnString(string IDTargetsRawData)
    {
        List<string> returnValue = new List<string>();
        returnValue.AddRange(IDTargetsRawData.Split('|'));
        return returnValue;
    }

    public IEnumerator PostToFacebook(string imageURL)
    {
        if (!Facebook.Unity.FB.IsInitialized)
        {
            FacebookShare.FBInit();
            //yield return new WaitUntil(FacebookShare.initialized);
        }
        if (!Facebook.Unity.FB.IsLoggedIn)
        {
            FacebookShare.LogIn();
            //yield return new WaitUntil(FacebookShare.loggedIn);
        }
        yield return new WaitForSeconds(10);
        FacebookShare.Share(imageURL);
    }    
}