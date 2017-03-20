using UnityEngine;
using Vuforia;
public class SetAutoFocus : MonoBehaviour {

    // Use this for initialization
    void Awake()
    {
        bool focusModeSet = CameraDevice.Instance.SetFocusMode(
        CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

        if (!focusModeSet)
        {
            Debug.Log("Failed to set focus mode (unsupported mode).");
        }
    }
	
}
