using UnityEngine;
using System.Collections;

public class SwipeToggle : MonoBehaviour {

    public Color InactiveColor;
    public Color activeColor;

    public void SetToOn()
    {
        GetComponent<UnityEngine.UI.Toggle>().isOn = true;
    }

    public void SetToOff()
    {
        UnityEngine.UI.Toggle toggleComponent = GetComponent<UnityEngine.UI.Toggle>();
        toggleComponent.isOn = true;
        //toggleComponent.colors.normalColor = activeColor;

    }
}
