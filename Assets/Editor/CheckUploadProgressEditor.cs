using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(CheckUploadProgress))]
public class CheckUploadProgressEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CheckUploadProgress targetScript = (CheckUploadProgress)target;
        if(GUILayout.Button("Check Default Info"))
        {
            targetScript.GetCheckTargetIDInfo();
        }

    }    
}
