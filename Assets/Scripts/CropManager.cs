using UnityEngine;
using System.Collections;

/// <summary>
/// The main crop manager.
/// Handles setting the crop area texture - starting the crop image command and 
/// </summary>
public class CropManager : MonoBehaviour
{
    public static CropManager instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindGameObjectWithTag("CropManager").GetComponent<CropManager>();
            return _instance;
        }
    }
    private static CropManager _instance;
    public UnityEngine.UI.RawImage cropImageArea;
    /// <summary>
    /// Set the Crop Area buttons in here. 
    /// [0] = Top Right Button
    /// [1] = Bottom Left Button
    /// </summary>
    public UnityEngine.UI.Button[] cropButtons;
    public RectTransform mainCropCanvas;
    public Camera cropCamera;
    /// <summary>
    /// This has to be set to the master Canvas Object so the UI can be faded before taking a screenshot
    /// </summary>
    public CanvasGroup CropUI;
    public Texture2D cropTexture;

    // Use this for initialization
    void Awake()
    {
        CropUI.alpha = 1;
        _instance = this;
    }

    void OnEnable()
    {
        CropUI.alpha = 1;
        _instance = this;
        // call backs
        ScreenshotManager.OnScreenshotTaken += ScreenshotTaken;
        ScreenshotManager.OnScreenshotSaved += ScreenshotSaved;
    }    

    void OnDisable()
    {
        _instance = this;
        //ScreenshotManager.OnScreenshotTaken -= ScreenshotTaken;
        //ScreenshotManager.OnScreenshotSaved -= ScreenshotSaved;
    }


    public void SetCropTexture(Texture2D texture)
    {
        cropTexture = texture;
        cropImageArea.texture = cropTexture;
    }

    public GameObject DebugPanel;

    public void CropImage()
    {
        CropUI.alpha = 0;
        Vector2 sizeCrop = CalculateSizeCropArea();

        if (sizeCrop.x < 0 || sizeCrop.y < 0)
        {
            Debug.Log("Not a valid Size");
            CropUI.alpha = 1;
            DebugPanel.GetComponentInChildren<UnityEngine.UI.Text>().text = "Not a valid Size";
            DebugPanel.SetActive(true);
        }
        else
            StartCoroutine(SaveScreenshotPause());
    }

    public IEnumerator SaveScreenshotPause()
    {
        /*DebugPanel.GetComponentInChildren<UnityEngine.UI.Text>().text = "Saving Screenshot";
        DebugPanel.SetActive(true);*/
        yield return new WaitForFixedUpdate();
        ScreenshotManager.SaveScreenshot("Augmented Target", "Augmented Targets", "jpeg", new Rect(RectTransformUtility.WorldToScreenPoint(cropCamera, cropButtons[1].transform.position), CalculateSizeCropArea()));
    }

    public void LogCalculatedCropArea()
    {
        Debug.Log(" CalculateSizeCropArea() = " + CalculateSizeCropArea());
    }

    Vector2 CalculateSizeCropArea()
    {
        Vector2 screenCoordBottomLeft = RectTransformUtility.WorldToScreenPoint(cropCamera, cropButtons[1].transform.position);
        Vector2 screenCoordTopRight = RectTransformUtility.WorldToScreenPoint(cropCamera, cropButtons[0].transform.position);
        Vector2 size = new Vector2(screenCoordTopRight.x - screenCoordBottomLeft.x, screenCoordTopRight.y - screenCoordBottomLeft.y);
        Debug.Log("Calculated Size" + size);
        return size;
    }    

    void ScreenshotTaken(Texture2D image)
    {
        Debug.Log("\nScreenshot has been taken and is now saving...");
        /*DebugPanel.GetComponentInChildren<UnityEngine.UI.Text>().text = "Loading Preview";
        DebugPanel.SetActive(true);*/
        VWSCloudConnecter.instance.SetPreviewImage(image);
        //SetCropTexture(image);
        SceneManager.LoadIngameScene("PreviewWindow"); //TODO Abstract to find scene names crop - set to static to resolve bug which said referenced object not available
    }

    string pathImageSaved;
    void ScreenshotSaved(string path)
    {        
        pathImageSaved = path;
        try {
            CropUI.alpha = 1;
        }
        catch
        {
            Debug.Log("CropUI Doesn't exist anymore");
        }
        //Activate Next Scene
    }
}
