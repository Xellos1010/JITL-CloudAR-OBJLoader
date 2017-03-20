using UnityEngine;

/// <summary>
/// This class handles Anchor touch events
/// </summary>
[System.Serializable]
public class ButtonTouchedData : MonoBehaviour
{
    /// <summary>
    /// The bounding area for the anchor to move within
    /// </summary>
    public RectTransform boundingCanvas;
    /// <summary>
    /// The RectTransform for the Anchor
    /// </summary>
    public RectTransform buttonTrackedRect;
    /// <summary>
    /// The UI Button affected
    /// </summary>
    public UnityEngine.UI.Button buttonTracked;

    public Vector2 pointerOffset;
}