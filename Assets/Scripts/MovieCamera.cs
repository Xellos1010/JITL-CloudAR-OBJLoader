using UnityEngine;
using System.Collections;

public class MovieCamera : MonoBehaviour
{
    private RenderTexture renderTexture = null;
    private Texture2D videoFrame;
    private Rect rect;   // Rect from where we need to read the pixels(entire  screen)

    void Start()
    {

    }

    void Update()
    {
    }

    void OnPreRender()
    {
        if (renderTexture == null)
        {
            Create();
        }
    }

    public void TakePhoto()
    { 
        // make camera’s targetTexture as current rendering target
        // and then read the pixels.
        RenderTexture orig = RenderTexture.active;
        RenderTexture.active = renderTexture;
        videoFrame.ReadPixels(rect, 0, 0, false);
        videoFrame.Apply();
        RenderTexture.active = orig;
    }

    void OnGUI()
    {
        // draw the read texture on to the screen
        Graphics.Blit(videoFrame, (RenderTexture)null);
    }

    private void Create()
    {
        Debug.Log("Creating Render Texture");
        renderTexture = new RenderTexture(Screen.width, Screen.height, 8, RenderTextureFormat.ARGB32);
        videoFrame = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

        // set targetTexture for the Camera
        GetComponent<Camera>().targetTexture = renderTexture;

        // Rect to read the pixels; entire screen
        rect = new Rect(0, 0, Screen.width, Screen.height);
    }
}