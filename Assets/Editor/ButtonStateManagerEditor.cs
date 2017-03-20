using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ButtonStateManager))]
public class ButtonStateManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ButtonStateManager targetScript = (ButtonStateManager)target;
        if(GUILayout.Button("Set State To Pending"))
        {
            targetScript.ActivatePendingState();
        }
        if (GUILayout.Button("Stop Pending"))
        {
            targetScript.DeactivatePendingState();
        }
        if (GUILayout.Button("Set Active State"))
        {
            targetScript.ActivateActiveColor();
        }
    }    
}
