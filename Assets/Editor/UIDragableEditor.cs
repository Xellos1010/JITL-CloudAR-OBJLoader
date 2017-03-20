using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UIDragInArea))]
public class UIDraggableEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIDragInArea targetScript = (UIDragInArea)target;
        if (GUILayout.Button("Set Script Data"))
        {
            targetScript.SetScriptData();
        }
        /*if (GUILayout.Button("Check Clamp Area"))
        {
            targetScript.ClampToWindowAndSetPos();
        }*/
    }
}
