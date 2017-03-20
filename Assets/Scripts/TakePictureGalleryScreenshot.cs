using UnityEngine;
using Vuforia;

public class TakePictureGalleryScreenshot : MonoBehaviour
{
    public Texture2D imageSavedGallery;
    public CropManager cropManager;

    void OnEnable()
    {
        // call backs
        ScreenshotManager.OnScreenshotTaken += ScreenshotTaken;
        ScreenshotManager.OnScreenshotSaved += ScreenshotSaved;
    }

    void OnDisable()
    {
        ScreenshotManager.OnScreenshotTaken -= ScreenshotTaken;
        ScreenshotManager.OnScreenshotSaved -= ScreenshotSaved;
    }

    public void TakePhoto()
    {
        //Gallery Grab Code
        if (!takingScreenshot)
        {
            SceneManager.AlphaManagerCanvasGroup.alpha = 0;
            ToggleMenuBarAlpha(false);
            takingScreenshot = true;
            ScreenshotManager.SaveScreenshot("Augmented Target", "Augmented Targets", "jpeg");
        }
    }    

    bool takingScreenshot = false;
    void ScreenshotTaken(Texture2D image)
    {
        Sprite screenShotTakenGallery;
        imageSavedGallery = image;
        Debug.Log("\nScreenshot has been taken and is now saving...");
        screenShotTakenGallery = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(.5f, .5f));
        //CropManager.instance.SetCropTexture(image);
        cropManager.SetCropTexture(image);
        ToggleUI(true);
        
        Debug.Log(SceneManager.InGameScenes.ToString());
        //SceneManager.LoadIngameScene(CropManager.instance.gameObject.name);   
        takingScreenshot = false;    
        SceneManager.LoadIngameScene(cropManager.gameObject.name);
    }    
    
    string pathImageSaved;
    void ScreenshotSaved(string path)
    {
        pathImageSaved = path;
    }

    void ToggleUI(bool onOff)
    {
        if (onOff)
            SceneManager.AlphaManagerCanvasGroup.alpha = 1;
        else
            SceneManager.AlphaManagerCanvasGroup.alpha = 0;
        ToggleMenuBarAlpha(onOff);
    }

    void ToggleMenuBarAlpha(bool onOff)
    {
        try
        {
            GameObject[] SceneObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Main").GetRootGameObjects();
            foreach (GameObject rootObject in SceneObjects)
            {
                if (rootObject.name.Contains("Canvas"))
                    if (onOff)
                        rootObject.GetComponent<CanvasGroup>().alpha = 1;
                    else
                        rootObject.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

}