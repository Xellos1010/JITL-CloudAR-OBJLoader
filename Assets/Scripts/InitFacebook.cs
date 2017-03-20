using UnityEngine;
using System.Collections;

public class InitFacebook : MonoBehaviour {

	// Use this for initialization
	void Awake() {
        FacebookShare.FBInit();
        FacebookShare.LogIn();
    }
	
}
