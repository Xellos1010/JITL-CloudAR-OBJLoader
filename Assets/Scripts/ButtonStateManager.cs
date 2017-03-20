using UnityEngine;
using System.Collections;

public class ButtonStateManager : MonoBehaviour {

    public bool pendingActive = false;
    UnityEngine.UI.Image lightImage
    {
        get
        {
            if (_lightImage == null)
                _lightImage = GetComponentInChildren<UnityEngine.UI.Image>();
            return _lightImage;
        }
    }
    UnityEngine.UI.Image _lightImage;


    public MenuBarSceneLoader sceneLoader;

    public void ActivatePendingState()
    {
        Debug.Log("Activating Pending State for " + gameObject.name);
        ActivatePendingState(sceneLoader.pendingButtonColor, sceneLoader.inactiveButtonColor, sceneLoader.pendingColorBlinkDuration);
    }

    public void ActivatePendingState(Color pendingColor, Color inActiveColor, float pendingBlinkDuration)
    {
        Debug.Log("Activating Pending State for " + gameObject.name);
        pendingActive = true;
        StartCoroutine(FlashPending(pendingColor, inActiveColor,pendingBlinkDuration));
    }

    public void DeactivatePendingState()
    {
        pendingActive = false;
        StopAllCoroutines();
    }

    public void FlashPendingColor(Color pendingColor, Color inActiveColor, float pendingBlinkDuration)
    {
        StartCoroutine(FlashPending(pendingColor, inActiveColor, pendingBlinkDuration));
    }

    IEnumerator FlashPending(Color pendingColor, Color inActiveColor, float pendingBlinkDuration)
    {
        Debug.Log("Flashing Pending Color");
        ChangeColor(pendingColor);
        yield return new WaitForSeconds(pendingBlinkDuration);
        ChangeColor(inActiveColor);
        yield return new WaitForSeconds(pendingBlinkDuration);
        StartCoroutine(FlashPending(pendingColor, inActiveColor, pendingBlinkDuration));
    }

    public void StopFlashing()
    {
        StopAllCoroutines();
    }    

    public void ActivateActiveColor()
    {
        ChangeColor(sceneLoader.activeButtonColor);
    }

    public void ChangeColor(Color color)
    {
        lightImage.color = color;
        //TODO Fade Color
    }

    void FadeColor()
    {

    }
}
