using UnityEngine;
using System.Collections;

public class MoveToLoop : MonoBehaviour {

    public Vector3 moveFrom;
    public Vector3 moveTo;
    public float moveFromY;
    public float moveToY;
    public float alphaFrom;
    public float alphaTo;

    public float time = .5f;

    RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }
    private RectTransform _rectTransform;

    UnityEngine.UI.RawImage rawImage
    {
        get
        {
            if (_rawImage == null)
                _rawImage = GetComponent<UnityEngine.UI.RawImage>();
            return _rawImage;
        }
    }
    private UnityEngine.UI.RawImage _rawImage;



    void Awake()
    {
        UpdateY(moveFromY);
        UpdateAlpha(alphaFrom);
    }

	// Use this for initialization
	void OnEnable()
    {
        UpdateY(moveFromY);
        UpdateAlpha(alphaFrom);
        StartTweenMovement();
        StartTweenAlpha();
	}

    void OnDisable()
    {
        StopTween();
    }	

    public void StopTween()
    {
        iTween.Stop();
    }

    public void StartTweenMovement()
    {
        iTween.ValueTo(gameObject, tweenParamsStartMovement());
    }

    Hashtable tweenParamsStartMovement()
    {
        Hashtable returnValue = new Hashtable();
        returnValue.Add("from", moveFromY);
        returnValue.Add("to", moveToY);
        returnValue.Add("time", time);
        returnValue.Add("easetype", iTween.EaseType.easeInQuad);
        returnValue.Add("onupdate", "UpdateY");
        returnValue.Add("onupdatetarget", gameObject);
        returnValue.Add("oncomplete", "ResetTweenMovement");
        returnValue.Add("oncompletetarget", gameObject);
        return returnValue;
    }

    public void ResetTweenMovement()
    {
        iTween.ValueTo(gameObject, tweenParamsResetMovement());
    }

    Hashtable tweenParamsResetMovement()
    {
        Hashtable returnValue = new Hashtable();
        returnValue.Add("from", moveToY);
        returnValue.Add("to", moveFromY);
        returnValue.Add("time", time);
        returnValue.Add("easetype", iTween.EaseType.easeOutQuad);
        returnValue.Add("onupdate", "UpdateY");
        returnValue.Add("onupdatetarget", gameObject);
        returnValue.Add("oncomplete", "StartTweenMovement");
        returnValue.Add("oncompletetarget", gameObject);
        return returnValue;
    }

    public void StartTweenAlpha()
    {
        iTween.ValueTo(gameObject, tweenParamsStartAlpha());
    }

    Hashtable tweenParamsStartAlpha()
    {
        Hashtable returnValue = new Hashtable();
        returnValue.Add("from", alphaFrom);
        returnValue.Add("to", alphaTo);
        returnValue.Add("time", time);
        returnValue.Add("easetype", iTween.EaseType.easeInQuad);
        returnValue.Add("onupdate", "UpdateAlpha");
        returnValue.Add("onupdatetarget", gameObject);
        returnValue.Add("oncomplete", "ResetTweenAlpha");
        returnValue.Add("oncompletetarget", gameObject);
        return returnValue;
    }

    public void ResetTweenAlpha()
    {
        iTween.ValueTo(gameObject, tweenParamsResetAlpha());
    }

    Hashtable tweenParamsResetAlpha()
    {
        Hashtable returnValue = new Hashtable();
        returnValue.Add("from", alphaTo);
        returnValue.Add("to", alphaFrom);
        returnValue.Add("time", time);
        returnValue.Add("easetype", iTween.EaseType.easeOutQuad);
        returnValue.Add("onupdate", "UpdateAlpha");
        returnValue.Add("onupdatetarget", gameObject);
        returnValue.Add("oncomplete", "StartTweenAlpha");
        returnValue.Add("oncompletetarget", gameObject);
        return returnValue;
    }

    public void UpdateY(float newValue)
    {
        rectTransform.position = new Vector3(rectTransform.position.x,newValue, rectTransform.position.z);
    }

    public void UpdateAlpha(float newValue)
    {
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, newValue);
    }

    public void SetMoveToPosition()
    {
        moveTo = rectTransform.position;
        moveToY = rectTransform.position.y;
    }

    public void SetMoveFromPosition()
    {
        moveFrom = rectTransform.position;
        moveFromY = rectTransform.position.y;
    }

    public void SetAlphaFromValue()
    {
        alphaFrom = rawImage.color.a;
    }

    public void SetAlphaToValue()
    {
        alphaTo = rawImage.color.a;
    }

    public void SetAlphaToAlphaFromValue()
    {
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b,alphaFrom);
    }

    public void SetAlphaToAlphaToValue()
    {
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, alphaTo);
    }

    public void MoveToFromPosition()
    {
        rectTransform.position = moveFrom;
    }
}
