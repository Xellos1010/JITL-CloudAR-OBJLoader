using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SceneObject))]
public class SceneObjectEditorUI : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneObject targetScript = (SceneObject)target;
        if(GUILayout.Button("Focus On Scene"))
        {
            try
            {
                targetScript.transform.parent.GetComponent<InGameSceneManager>().FocusSceneActive(targetScript);
            }
            catch(System.Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log("Make sure the parent transform of " + targetScript.gameObject.name + " has InGameSceneManager Component Attached");
            }
        }
    }    
}
