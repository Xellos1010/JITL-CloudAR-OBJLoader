using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Set a UI element to be draggable within a Canvas Area
/// </summary>
public class UIDragInArea : MonoBehaviour, IPointerDownHandler
{
    bool adjustAnchor = false;
    int assignedFingerID = -1;
    public RectTransform buttonRect;
    public ButtonTouchedData touchData;
    public bool useHalfScreen = false;
    public bool topRight = false;

    void Awake()
    {
        buttonRect = GetComponent<RectTransform>();
        touchData = GetComponent<ButtonTouchedData>();
    }

    public void SetScriptData()
    {
        buttonRect = GetComponent<RectTransform>();
        touchData = GetComponent<ButtonTouchedData>();
    }

    void Update()
    {
        if (adjustAnchor)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(assignedFingerID).phase == TouchPhase.Ended || Input.GetTouch(assignedFingerID).phase == TouchPhase.Canceled)
                    DeactivateAnchor();
                else
                {
                    Vector2 pointerPostion;
                    Vector2 localPointerPosition;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        CropManager.instance.mainCropCanvas,
                        Input.GetTouch(assignedFingerID).position,
                        CropManager.instance.cropCamera,
                        out localPointerPosition))
                    {
                        if (touchData.boundingCanvas)
                            UtilityFunctions.ClampToWindow(
                                localPointerPosition,
                                touchData.boundingCanvas,
                                out pointerPostion);
                        else
                            pointerPostion = localPointerPosition;

                        Vector2 newPos;
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            CropManager.instance.mainCropCanvas,
                            pointerPostion,
                            null,
                            out newPos))
                        {
                            Vector2 finalResult = pointerPostion - pointerOffset;
                            buttonRect.localPosition = new Vector3(
                                buttonRect.localPosition.x + finalResult.x,
                                buttonRect.localPosition.y + finalResult.y,
                                0);
                            pointerOffset = pointerOffset + finalResult;
                        }
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 pointerPostion;
                Debug.Log(Input.mousePosition);
                Vector2 localPointerPosition;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(CropManager.instance.mainCropCanvas, Input.mousePosition, CropManager.instance.cropCamera, out localPointerPosition))
                {
                    Debug.Log("localPointerPosition = " + localPointerPosition);
                    if (touchData.boundingCanvas)
                        UtilityFunctions.ClampToWindow(localPointerPosition, touchData.boundingCanvas, out pointerPostion);
                    else
                        pointerPostion = localPointerPosition;
                    //Debug.Log("localPointerPosition after crop= " + pointerPostion);
                    Debug.Log("pointerPostion - pointerOffset = " + (pointerPostion - pointerOffset));
                    Vector2 finalResult = pointerPostion - pointerOffset;
                    buttonRect.localPosition = new Vector3(buttonRect.localPosition.x + finalResult.x, buttonRect.localPosition.y + finalResult.y, 0);
                    pointerOffset = pointerOffset + finalResult;
                }

            }
            else
                DeactivateAnchor();
        }
    }
    
    public void InitializeCropAnchor()
    {
        adjustAnchor = true;
        if (Input.touchCount > 0)
        {
            foreach (Touch inputTouch in Input.touches)
            {
                if (inputTouch.phase == TouchPhase.Began)
                {
                    assignedFingerID = inputTouch.fingerId;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(CropManager.instance.mainCropCanvas, inputTouch.position, CropManager.instance.cropCamera, out pointerOffset);
                }
            }
        }        
    }

    public Vector2 pointerOffset;

    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log("Pointer Down Acknowledged");
        Debug.Log("data.position = " + data.position);
        Debug.Log("Input.mousePosition = " + Input.mousePosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CropManager.instance.mainCropCanvas, data.position, CropManager.instance.cropCamera, out pointerOffset);
        Debug.Log("pointerOffset = " + pointerOffset);
        Debug.Log("buttonRect.localPosition = " + buttonRect.localPosition);
        Debug.Log("buttonRect.anchoredPosition = " + buttonRect.anchoredPosition);
    }

    public void OnDrag(PointerEventData data)
    {
        Debug.Log("Drag Acknowledged");
        if (buttonRect == null)
            return;

        Vector2 pointerPostion =  ClampToWindow(data.position);

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CropManager.instance.mainCropCanvas, pointerPostion, data.pressEventCamera, out localPointerPosition
        ))
        {
            buttonRect.localPosition = localPointerPosition - pointerOffset;
        }
    }

    Vector2 ClampToWindow(Vector2 dataPosition)
    {
        Vector2 rawPointerPosition = dataPosition;

        Vector3[] canvasCorners = new Vector3[4];
        CropManager.instance.mainCropCanvas.GetWorldCorners(canvasCorners);

        float clampedX = Mathf.Clamp(rawPointerPosition.x, canvasCorners[0].x, canvasCorners[2].x);
        float clampedY = Mathf.Clamp(rawPointerPosition.y, canvasCorners[0].y, canvasCorners[2].y);

        Vector2 newPointerPosition = new Vector2(clampedX, clampedY);
        return newPointerPosition;
    }

    void DeactivateAnchor()
    {
        adjustAnchor = false;
        assignedFingerID = -1;

    }
}
