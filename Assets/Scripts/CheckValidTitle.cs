using UnityEngine;
using System.Collections;

public class CheckValidTitle : MonoBehaviour {

	// Use this for initialization
	public void CheckTitleAndContinueObjectSceneLoad(UnityEngine.UI.InputField title)
    {
	    if (title.text == "")
        {
            ActivateRequireField(title);
        }
        else
        {
            SceneManager.LoadIngameScene("Choose3DModel");
        }
	}

    void ActivateRequireField(UnityEngine.UI.InputField inputField)
    {
        inputField.placeholder.color = Color.red;
        //TODO if URL toggle selected and URL field has no text set placeholder for URL to red
    }
}
