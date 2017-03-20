using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CropManager))]
public class CropManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CropManager targetScript = (CropManager)target;
        if (GUILayout.Button("Test Size Calculation"))
        {
            targetScript.LogCalculatedCropArea();
        }
    }
}
