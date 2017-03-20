using UnityEngine;
using System.Collections;

public class WebVideoManager : MonoBehaviour {
    public static WebVideoManager instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindGameObjectWithTag("WebVideoManager").GetComponent<WebVideoManager>();
            return _instance;
        }
    }
    private static WebVideoManager _instance;

    //public MediaPlayerCtrl mediaPlayer;
    //public EasyWebViewCtrl webView;

    void Awake()
    {
        _instance = this;
    }
    
    // Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
