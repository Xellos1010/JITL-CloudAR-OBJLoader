using UnityEngine;
using System.Collections;

public class ManageTargetMainManager : GameManager {
    public GameObject areYouSure;
    public UnityEngine.UI.InputField[] fieldToClear;
    public WebCamTexturePlayer webCamPlayer;
	// Use this for initialization
	void Start ()
    {
        if (areYouSure != null)
            areYouSure.SetActive(false);
	}

    void ActivateWebCamTexture()
    {
        webCamPlayer.webcamTexture.Play();
    }

    void ClearInputFields()
    {
        foreach (UnityEngine.UI.InputField input in fieldToClear)
        {
            input.text = "";
        }
    }

    public void CheckAreYouSure()
    {
        areYouSure.SetActive(true);
    }

    public void ActivateIngameScene(GameObject Scene)
    {

    }

    public void AreYouSure(bool yesNo)
    {
        if (yesNo)
        {            
            VWSCloudConnecter.instance.DeleteAllTargetIDs();
        }
        areYouSure.SetActive(false);
    }
}
