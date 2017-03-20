/*==============================================================================
Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
==============================================================================*/
using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Threading;
using Vuforia;

/// <summary>
/// This class manages the content displayed on top of cloud reco targets in this sample
/// </summary>
public class ContentManager : MonoBehaviour, ITrackableEventHandler
{
    private string urlToLoad = "";
    public YoutubeVideo youtubeObject;
    public MediaPlayerCtrl mediaController;
    public UnityEngine.UI.Text urlTitle;

    #region PUBLIC_MEMBERS
    /// <summary>
    /// The root gameobject that serves as an augmentation for the image targets created by search results
    /// </summary>
    //public GameObject AugmentationObject;

    public ObjectManager objectManager; // This should be assigned to the Cloud Reco Target and either cloud reco target has the object manager assigned to it or the object manager is set on a different gameobject
    #endregion //PUBLIC_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    void Awake()
    {
        CheckObjectManagerAssigned();
        TrackableBehaviour trackableBehaviour = null;
        try
        {
            trackableBehaviour = GameObject.FindGameObjectWithTag("CloudRecoTarget").GetComponent<TrackableBehaviour>();
        }
        catch(Exception e)
        {
            if (e.Message.Contains("reference not set"))
            {
                trackableBehaviour = objectManager.GetComponent<TrackableBehaviour>();
            }
            else
            {
                Debug.Log(e.Message);
            }
        }
        

            //AugmentationObject.transform.parent.GetComponent<TrackableBehaviour>();
        if (trackableBehaviour)
        {
            trackableBehaviour.RegisterTrackableEventHandler(this);
        }

        ToggleObjects(false);//ShowObject(false);
    }

    void CheckObjectManagerAssigned()
    {
        if (objectManager == null)
            try
            {
                objectManager = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>();
            }
            catch(Exception e)
            {
                if (e.Message.Contains("reference not set"))
                {
                    Debug.Log("object Manager needs to be set in Content Manager or Object with Objectmanager should be assigned the tag ObjectManager");
                    objectManager = FindObjectOfType<ObjectManager>();
                }
            }
    }
    #endregion MONOBEHAVIOUR_METHODS and Support Methods


    #region PUBLIC_METHODS
    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED || 
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            ToggleObjects(false);
            //ToggleObject(UnityEngine.Random.Range(0, (objectManager.transform.childCount - 1)),true);
        }
        else
        {
            ToggleObjects(false);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ToggleObject(UnityEngine.Random.Range(0, (objectManager.transform.childCount - 1)), true);
        if(Input.GetKeyDown(KeyCode.S))
        {
            ToggleObjects(false);
        }
    }

    public void ToggleObjects(bool tf)
    {
        objectManager.ToggleObjects(tf);
    }

    public void ToggleObject(int augmentObject, bool tf)
    {
        objectManager.ToggleObjectWithWait(augmentObject, tf);
    }

    public void ToggleObject(GameObject augmentObject, bool tf)
    {
        objectManager.ToggleObjectWithWait(augmentObject, tf);
    }

    public void PassMetaData(string metaData)
    {
        JSONObject meta = new JSONObject(metaData);
        Debug.Log(meta["ContentType"] + "=ContentType");
        //TODO Add Switch to Content Object Manager to hold Video and Website Button and switch to specific content
        string str = meta["ContentType"].ToString();
        //Choose all numbers and letters and remove ""
        str = new string((from c in str
                          where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                          select c
               ).ToArray());
        ContentTypeEnum contentEnum = ParseEnum<ContentTypeEnum>(str);
        switch (contentEnum)
        {
            case ContentTypeEnum.Object:
                Debug.Log("Object Being Activated" + meta["objectName"].ToString());
                string objectName = meta["objectName"].ToString();
                objectName = new string((from c in objectName
                                         where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                                  select c
               ).ToArray());
                ToggleObject(objectManager.transform.FindChild(objectName).gameObject, true);
                break;
            case ContentTypeEnum.Website:
                Debug.Log("Website Being Activated");
                //Set URL to load
                string urlWeb = meta["urlWeb"].ToString();
                urlWeb = urlWeb.Trim('"');
                    /*new string((from c in urlWeb
                                     where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c) || c == '.' || c == ':' || c == '/'
                                     select c
               ).ToArray());*/
                Debug.Log(urlWeb);
                //ToggleObject(objectManager.transform.FindChild("websiteLoad").gameObject, true);
                urlToLoad = urlWeb;
                SetURLTitle();
                LoadURLButton();                
                break;
            case ContentTypeEnum.Video:
                Debug.Log("Video Being Activated");                
                string urlVideo = meta["urlVideo"].ToString();
                urlVideo = urlVideo.Trim('"');
                    /*new string((from c in urlVideo
                                    where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c) || c == '.' || c == ':' || c == '/' || c == '?' || c == '='
                                       select c
               ).ToArray());*/
                urlToLoad = urlVideo;
                StartCoroutine(PlayVideo(360));

                //Pass the youtube video id to RequestVideo in YoutubeVideo and the system returns the video .mp4 file.                 
                //string videoURL = youtubeObject.RequestVideo(urlToLoad,480);

                //yield return youtubeObject.RequestVideo(urlToLoad, 480);

                //Call the unity3D Handheld.PlayFullScreenMovie to play your video in fullscreen.
                //Handheld.PlayFullScreenMovie(videoURL);

                //ToggleObject(objectManager.transform.FindChild("videoManager").gameObject, true);
                break;
            default:
                Debug.Log("Content Type from meta Data passed not a Valid type. Type passed from server = " + str + " Please add valid content type to switch in ContentManager.cs");
                throw new Exception("Content Type from meta Data passed not a Valid type. Type passed from server = " + str + " Please add valid content type to switch in ContentManager.cs");
        }
    }

    public UnityEngine.UI.Text loadingText;

    IEnumerator PlayVideo(int quality)
    {
        string videoURL;

        //Load the In-Game scene to load the video. Using this for a smoother user experience since RequestVideo URL Takes a while to load
        InGameSceneManager.instance.LoadIngameScene("VideoLoader");

        //Use this to place easy movie texture on the cloud target.
        //ToggleObject(objectManager.transform.FindChild("videoManager").gameObject, true);
        yield return new WaitForSeconds(1);

        //Pass the youtube video id to RequestVideo in YoutubeVideo and the system returns the video .mp4 file.
        yield return youtubeObject.RequestVideo(urlToLoad, quality, out videoURL);

        //Call the unity3D Handheld.PlayFullScreenMovie to play your video in fullscreen.
        mediaController.Load(videoURL);
        //Debug.Log(MediaPlayerCtrl.instance.gameObject.name);
        MediaPlayerCtrl.instance.Load(videoURL);
        //yield return new WaitForSeconds(2f);
        loadingText.text = "Press play to play video";
        //mediaController.Play();
        //Handheld.PlayFullScreenMovie(videoURL);
    }

    T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public void SetURLTitle()
    {
        urlTitle.text = urlToLoad;
    }

    public void LoadURLButton()
    {
        Debug.Log("Load URL was submitted");
        //TODO Toggle off augmented objects on application load
        objectManager.ToggleObjects(false);
        ToggleObject(objectManager.transform.FindChild("websiteLoad").gameObject, true);
        //TODO URL char removal more specific for ""        
    }

    public void LoadURL()
    {
        objectManager.ToggleObjects(false);
        Application.OpenURL(urlToLoad);
    }

    IEnumerator WaitAndPlayVideo(float time, JSONObject meta)
    {
        //WebVideoManager.instance.mediaPlayer.Load(meta["urlVideo"].ToString().Replace("\"", ""));
        yield return new WaitForSeconds(time);
        //WebVideoManager.instance.mediaPlayer.m_bLoop = true;
        //WebVideoManager.instance.mediaPlayer.m_strFileName = meta["urlVideo"].ToString().Replace("\"", "");
        //yield return new WaitForSeconds(time);
        //WebVideoManager.instance.mediaPlayer.Play();
    }

    /*public void ToggleObjects(bool tf)
    {
        ToggleObjects(tf);
        /*
        Renderer[] rendererComponents = AugmentationObject.GetComponentsInChildren<Renderer>();
        Collider[] colliderComponents = AugmentationObject.GetComponentsInChildren<Collider>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = tf;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = tf;
        }
        
    }*/ 
    #endregion //PUBLIC_METHODS
}
