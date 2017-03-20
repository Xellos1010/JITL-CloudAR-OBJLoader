using UnityEngine;
using Vuforia;
using System.Collections;

public class WebCamTexturePlayer : MonoBehaviour
{
    public WebCamTexture webcamTexture;
    public UnityEngine.UI.RawImage rawimage;
    //Quaternion baseRotation;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        try
        {
            CrazyFocusPocus();
        }
        catch{ Debug.Log("Auto Focus not enabled"); }
        //baseRotation = transform.rotation;
        PlayWebcamTexture();            
    }

    void PlayWebcamTexture()
    {
        rawimage.texture = webcamTexture;
        rawimage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    void CrazyFocusPocus()
    {
#if UNITY_ANDROID

        // Get activity instance (standard way, solid)
        var pl_class = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = pl_class.GetStatic<AndroidJavaObject>("currentActivity");

        // Get instance of UnityPlayer (hacky but will live)
        var pl_inst = currentActivity.Get<AndroidJavaObject>("mUnityPlayer");

        // Get list of camera wrappers in UnityPlayer (very hacky, will die if D becomes C tomorrow)
        var list = pl_inst.Get<AndroidJavaObject>("D");
        var x = list.Call<int>("size");

        if (x == 0) return;

        // Get the first element of list (active camera wrapper)
        var cam_holder = list.Call<AndroidJavaObject>("get", 0);

        // get camera (this is totally insane, again if "a" becomes not-"a" one day )
        var cam = cam_holder.Get<AndroidJavaObject>("a");

        // Call my setup camera routine in Android plugin  (will set params and call autoFocus)
        var jc = new AndroidJavaClass("org.example.ScriptBridge.JavaClass");
        jc.CallStatic("enableAutofocus", new[] { cam });
#endif
    }

    private void Update()
    {
        if (webcamTexture.width < 100)
        {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // change as user rotates iPhone or Android:

        int cwNeeded = webcamTexture.videoRotationAngle;
        // Unity helpfully returns the _clockwise_ twist needed
        // guess nobody at Unity noticed their product works in counterclockwise:
        int ccwNeeded = -cwNeeded;

        // IF the image needs to be mirrored, it seems that it
        // ALSO needs to be spun. Strange: but true.
        if (webcamTexture.videoVerticallyMirrored) ccwNeeded += 180;

        // you'll be using a UI RawImage, so simply spin the RectTransform
        rawimage.transform.localEulerAngles = new Vector3(0f, 0f, ccwNeeded);

        float videoRatio = (float)webcamTexture.width / (float)webcamTexture.height;

        // you'll be using an AspectRatioFitter on the Image, so simply set it
        //rawimage .aspectRatio = videoRatio;

        // alert, the ONLY way to mirror a RAW image, is, the uvRect.
        // changing the scale is completely broken.
        if (webcamTexture.videoVerticallyMirrored)
            rawimage.uvRect = new Rect(1, 0, -1, 1);  // means flip on vertical axis
        else
            rawimage.uvRect = new Rect(0, 0, 1, 1);  // means no flip
        ccwNeeded = 90;
        CheckScaleTexture(ccwNeeded);
        // devText.text =
        //  videoRotationAngle+"/"+ratio+"/"+wct.videoVerticallyMirrored;
    }

    void CheckScaleTexture(float ccw)
    {
        if ((ccw == 90 || ccw == -90) && rawimage.rectTransform.sizeDelta != new Vector2(Screen.height, Screen.width))
            rawimage.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
    }
}
