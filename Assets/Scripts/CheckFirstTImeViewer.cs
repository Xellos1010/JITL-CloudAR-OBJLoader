using UnityEngine;
using System.Collections;

public class CheckFirstTImeViewer : MonoBehaviour {


	// Use this for initialization
	void Awake()
    {
        if (!PlayerPrefs.HasKey("ViewInstructions"))
            PlayerPrefs.SetString("ViewInstructions", "false");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
