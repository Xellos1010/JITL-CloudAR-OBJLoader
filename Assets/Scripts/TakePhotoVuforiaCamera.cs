using UnityEngine;
using Vuforia;
using System.Collections;
using System.IO;

public class TakePhotoVuforiaCamera : MonoBehaviour, ITrackerEventHandler
{
    private Image.PIXEL_FORMAT m_PixelFormat = Image.PIXEL_FORMAT.RGBA8888;//Image.PIXEL_FORMAT.RGB888;
    private bool m_RegisteredFormat = false;
    private bool m_LogInfo = true;
    private bool paused = false;
    public Texture2D imageCaptured;

    void Start()
    {
        VuforiaBehaviour qcarBehaviour = (VuforiaBehaviour)FindObjectOfType(typeof(VuforiaBehaviour));
        if (qcarBehaviour)
        {
            qcarBehaviour.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        }
    }

    public void OnInitialized()
    {

    }

    public void OnTrackablesUpdated()
    {
        if (!m_RegisteredFormat)
        {
            CameraDevice.Instance.SetFrameFormat(m_PixelFormat, true);
            m_RegisteredFormat = true;
        }
        if (m_LogInfo)
        {
            CameraDevice cam = CameraDevice.Instance;
            Image image = cam.GetCameraImage(m_PixelFormat);
            if (image == null)
            {
                Debug.Log(m_PixelFormat + " image is not available yet");
            }
            else
            {
                string s = m_PixelFormat + " image: \n";
                s += "  size: " + image.Width + "x" + image.Height + "\n";
                s += "  bufferSize: " + image.BufferWidth + "x" + image.BufferHeight + "\n";
                s += "  stride: " + image.Stride;
                Debug.Log(s);
                m_LogInfo = false;
            }
        }
    }

    public void TakeScreenshot()
    {
        CameraDevice cam = CameraDevice.Instance;
        Image image = cam.GetCameraImage(m_PixelFormat);
        if (image == null)
        {
            Debug.Log(m_PixelFormat + " image is not available yet");
        }
        else
        {
            string s = m_PixelFormat + " image: \n";
            s += "  size: " + image.Width + "x" + image.Height + "\n";
            s += "  bufferSize: " + image.BufferWidth + "x" + image.BufferHeight + "\n";
            s += "  stride: " + image.Stride;
            Debug.Log(s);

            Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.RGB24, false);
            image.CopyToTexture(tex);
            tex.Apply();
            //Vuforia camera images come directly from the camera so you need to 
            tex = UtilityFunctions.SaveAsFlippedTexture2D(tex, true,false);
            tex.Apply();

            CropManager.instance.SetCropTexture(tex);
            SceneManager.LoadIngameScene("Crop Window"); //TODO Abstract to find scene names crop - set to static to resolve bug which said referenced object not available
            //byte[] bytes = tex.EncodeToPNG();
            //Destroy(tex);

            // For testing purposes, also write to a file in the project folder
            //File.WriteAllBytes(Application.persistentDataPath + "/Screenshot.png", bytes);
            
            // now use Unity's built in
            //Application.CaptureScreenshot("UnityScreenshot.png");
        }
    }

    /*IEnumerator RotateandSetTexture(Texture TextureToFlip)
    {
        Texture2D tex = new Texture2D(TextureToFlip.width, TextureToFlip.height);
        yield return UtilityFunctions.SaveAsFlippedTexture2D(TextureToFlip, false, true);

    }*/
    
}
