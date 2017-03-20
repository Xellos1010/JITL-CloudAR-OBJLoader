using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(MoveToLoop))]
public class MoveToLoopEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MoveToLoop targetScript = (MoveToLoop)target;
        if(GUILayout.Button("Set Move To Location"))
        {
            try
            {
                targetScript.GetComponent<MoveToLoop>().SetMoveToPosition();
            }
            catch(System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        if (GUILayout.Button("Set Move From Location"))
        {
            try
            {
                targetScript.GetComponent<MoveToLoop>().SetMoveFromPosition();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        if (GUILayout.Button("Move To From Location"))
        {
            try
            {
                targetScript.GetComponent<MoveToLoop>().MoveToFromPosition();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        if (GUILayout.Button("Set Alpha To"))
        {
            try
            {
                targetScript.GetComponent<MoveToLoop>().SetAlphaToValue();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        if (GUILayout.Button("Set Alpha From"))
        {
            try
            {
                targetScript.GetComponent<MoveToLoop>().SetAlphaFromValue();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        if (GUILayout.Button("Set Alpha to Alpha From Value"))
        {
            try
            {
                targetScript.GetComponent<MoveToLoop>().SetAlphaToAlphaFromValue();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        if (GUILayout.Button("Set Alpha to Alpha To Value"))
        {
            try
            {
                targetScript.GetComponent<MoveToLoop>().SetAlphaToAlphaToValue();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }    
}
