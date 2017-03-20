using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InGameSceneManager))]
public class InGameSceneManagerEditorUI : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InGameSceneManager targetScript = (InGameSceneManager)target;
        if(GUILayout.Button("Activate All Scenes"))
        {
            targetScript.ToggleAllScenes(true);
        }
        if (GUILayout.Button("Focus Scenes Activate on Awake"))
        {
            targetScript.FocusAwakeScene();
        }
    }    
}
