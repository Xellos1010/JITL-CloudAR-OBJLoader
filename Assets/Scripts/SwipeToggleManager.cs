using UnityEngine;
using System.Collections;

public class SwipeToggleManager : MonoBehaviour {

	public void SetAllTogglesOff()
    {
        GetComponent<UnityEngine.UI.ToggleGroup>().SetAllTogglesOff();
    }

    public void DisableGoingBack()
    {
        gameObject.SetActive(false);
    }
}
