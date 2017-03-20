using UnityEngine;
using UnityEngine.UI;

public class ChooseExperienceToggleManager : MonoBehaviour
{
    public GameObject uploadUIBTN;
    public GameObject objectViewerUIBTN;
    public InputField urlInput;

    void Start()
    {
        HandleValueChangeYoutubeURL(true);
    }

    //Handles UI Visibility for URL UI. Change VWSCloudConnecter contentType to URL
    public void HandleValueChangeWebsiteURL(bool onOff)
    {
        if (onOff)
        {
            SwitchCloudConnecterContentType(ContentTypeEnum.Website);
            CheckURLActive(onOff);
            ChangeURLPlaceholderText("Enter Website URL...");
            CheckUploadBtnActive(onOff);
            CheckObjectBtnActive(!onOff);
        }
    }

    //Handles UI Visibility for Youtube URL UI. Change VWSCloudConnecter contentType to Video
    public void HandleValueChangeYoutubeURL(bool onOff)
    {
        if (onOff)
        {
            SwitchCloudConnecterContentType(ContentTypeEnum.Video);
            CheckURLActive(onOff);
            ChangeURLPlaceholderText("Enter Youtube Video Number v=?...");
            CheckUploadBtnActive(onOff);
            CheckObjectBtnActive(!onOff);
        }
    }

    //Handles UI visibility for 3D Objects UI. Change VWSCloudConnecter contentType to Video
    public void HandleValueChangeObjects(bool onOff)
    {
        if (onOff)
        {
            SwitchCloudConnecterContentType(ContentTypeEnum.Object);
            CheckURLActive(!onOff);
            CheckUploadBtnActive(!onOff);
            CheckObjectBtnActive(onOff);
        }
    }

    void SwitchCloudConnecterContentType(ContentTypeEnum contentType)
    {
        VWSCloudConnecter.instance.contentType = contentType;
    }

    void CheckURLActive(bool onOff)
    {
        if (urlInput.gameObject.activeSelf != onOff)
            urlInput.gameObject.SetActive(onOff);
    }

    void ChangeURLPlaceholderText(string text)
    {
        urlInput.placeholder.GetComponent<Text>().text = text;
        urlInput.text = "";        
    }

    void CheckUploadBtnActive(bool onOff)
    {
        if (uploadUIBTN.activeSelf != onOff)
            uploadUIBTN.SetActive(onOff);
    }

    void CheckObjectBtnActive(bool onOff)
    {
        if (objectViewerUIBTN.activeSelf != onOff)
            objectViewerUIBTN.SetActive(onOff);
    }

}
