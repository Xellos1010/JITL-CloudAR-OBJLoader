using UnityEngine;
using System.Collections;

public class MedaiPlayerSampleGUI : MonoBehaviour {
	
	public MediaPlayerCtrl scrMedia;

    public GameObject playButton;
    public GameObject pauseButton;
    public UnityEngine.UI.Text loadingText;


    public bool m_bFinish = false;
	// Use this for initialization
	void Start () {
		scrMedia.OnEnd += OnEnd;

	}

	
	// Update is called once per frame
	void Update () {
	
	}

    /*void OnGUI() {
		
	
		if( GUI.Button(new Rect(50,50,100,100),"Load"))
		{
			scrMedia.Load("EasyMovieTexture.mp4");
			m_bFinish = false;
		}*/
    /*
    if( GUI.Button(new Rect(50,200,100,100),"Play"))
    {
        scrMedia.Play();
        m_bFinish = false;
    }

    if( GUI.Button(new Rect(50,350,100,100),"stop"))
    {
        scrMedia.Stop();
    }

    if( GUI.Button(new Rect(50,500,100,100),"pause"))
    {
        scrMedia.Pause();
    }

    if(GUI.Button(new Rect(50,650,100,100),"Unload"))
    {
        scrMedia.UnLoad();
    }

    if(GUI.Button(new Rect(50,800,100,100), " " + m_bFinish))
    {

    }

    if(GUI.Button(new Rect(200,50,100,100),"SeekTo"))
    {
        scrMedia.SeekTo(10000);
    }


    if( scrMedia.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
    {
        if( GUI.Button(new Rect(200,200,100,100),scrMedia.GetSeekPosition().ToString()))
        {
            scrMedia.SetSpeed(2.0f);
        }

        if( GUI.Button(new Rect(200,350,100,100),scrMedia.GetDuration().ToString()))
        {
            scrMedia.SetSpeed(1.0f);
        }

        if( GUI.Button(new Rect(200,450,100,100),scrMedia.GetVideoWidth().ToString()))
        {

        }

        if( GUI.Button(new Rect(200,550,100,100),scrMedia.GetVideoHeight().ToString()))
        {

        }
    }

    if( GUI.Button(new Rect(200,650,100,100),scrMedia.GetCurrentSeekPercent().ToString()))
    {

    }


}*/


    public void LoadVideo(string streamLocation)
    {
        scrMedia.Load("streamLocation");
        m_bFinish = false;
        Play();
    }

    public void Play()
    {
        scrMedia.Play();
        m_bFinish = false;

        //Swap Pause for Play
        TogglePlayPause(false);
    }

    public void Pause()
    {
        scrMedia.Pause();

        //Swap Pause for Play Texture
        TogglePlayPause(true);
    }

    public void Stop()
    {
        scrMedia.Stop();
        InGameSceneManager.instance.LoadIngameScene("AugmentScene");
    }

    private void TogglePlayPause(bool playPause)
    {
        if (playPause)
        {
            playButton.SetActive(true);
            pauseButton.SetActive(false);
            loadingText.text = "Press play to Resume";
            loadingText.gameObject.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
            pauseButton.SetActive(true);
            loadingText.gameObject.SetActive(false);
        }
    }

    public void FastForward()
    {

    }

	
	void OnEnd()
	{
		m_bFinish = true;
        Stop();
    }
}
