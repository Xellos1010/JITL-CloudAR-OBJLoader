/*===============================================================================
Copyright (c) 2015 PTC Inc. All Rights Reserved.
 
Copyright (c) 2010-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class CloudRecoTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    public GameObject scanUI;
    public GameObject scanLine;

    #region PRIVATE_MEMBERS
    private TrackableBehaviour mTrackableBehaviour;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
    #endregion //MONOBEHAVIOUR_METHODS


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
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.UNKNOWN &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            // Ignore this specific combo
            return;
        }
        else
        {
            OnTrackingLost();
        }
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS
    private void OnTrackingFound()
    {
        ToggleScan(false);
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = true;
        }

        // Stop finder since we have now a result, finder will be restarted again when we lose track of the result
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (objectTracker != null)
        {
            objectTracker.TargetFinder.Stop();
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
    }

    private void OnTrackingLost()
    {
        ToggleScan(true);
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Disable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }

        // Disable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = false;
        }
		
        // Start finder again if we lost the current trackable
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (objectTracker != null)
		{
            objectTracker.TargetFinder.ClearTrackables(false);
            objectTracker.TargetFinder.StartRecognition();
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
    }

    public void ToggleScan(bool onOff)
    {
        ToggleScanLine(onOff);
        ToggleScanUI(onOff);
    }

    public void ToggleScanUI(bool onOff)
    {
        scanUI.SetActive(onOff);
    }

    public void ToggleScanLine(bool onOff)
    {
        try
        {
            scanLine.SetActive(onOff);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message + " for the scanLine");
        }
        
    }
    #endregion //PRIVATE_METHODS
}
