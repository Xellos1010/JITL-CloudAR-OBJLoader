using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(instructions))]
public class instructionsEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        instructions targetScript = (instructions)target;
        if(GUILayout.Button("Check Toggle Buttons Match Length Slides"))
        {
            targetScript.CheckToggleButtonsLengthMatchSlides();
        }
        if (GUILayout.Button("Set Active Slide"))
        {
            targetScript.SetActiveSlide();
        }
        if (GUILayout.Button("Next Slide"))
        {
            targetScript.NextSlide();
        }
        if (GUILayout.Button("Previous Slide"))
        {
            targetScript.PreviousSlide();
        }
    }    
}
