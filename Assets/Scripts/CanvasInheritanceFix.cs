using UnityEngine;
using System.Collections;

public class CanvasInheritanceFix : MonoBehaviour
{
    public Vector2 anchorMin;
    public Vector2 anchorMax;

    RectTransform canvas
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }
    // Use this for initialization
    void Awake()
    {
        SetCanvasAnchors();
        SetOffsets();
    }

    public void SetOffsets()
    {
        canvas.offsetMin = Vector2.zero;
        canvas.offsetMax = Vector2.zero;
    }

    public void SetCanvasAnchors()
    {
        canvas.anchorMin = anchorMin;
        canvas.anchorMax = anchorMax;
    }

    public void UseParameters()
    {
        anchorMin = canvas.anchorMin;
        anchorMax = canvas.anchorMax;
        //SetParameters();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
