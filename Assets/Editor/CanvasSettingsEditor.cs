using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CanvasInheritanceFix))]
public class CanvasSettingsEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CanvasInheritanceFix targetScript = (CanvasInheritanceFix)target;
        if(GUILayout.Button("Reset Canvas"))
        {
            targetScript.SetCanvasAnchors();
        }

        if (GUILayout.Button("Set Script Parameters"))
        {
            targetScript.UseParameters();
        }

        if (GUILayout.Button("Set Offsets"))
        {
            targetScript.SetOffsets();
        }

    }    
}
