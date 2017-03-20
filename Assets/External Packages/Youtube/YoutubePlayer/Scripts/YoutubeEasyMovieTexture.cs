using UnityEngine;
using System.Collections;

public class YoutubeEasyMovieTexture : MonoBehaviour
{

    public string youtubeVideoIdOrUrl;

	void Start () {
        //LoadYoutubeInTexture();
	}

    public void LoadYoutubeInTexture()
    {
        //ALERT If you are using EasyMovieTexture uncomment the line..
        this.gameObject.GetComponent<MediaPlayerCtrl>().m_strFileName = YoutubeVideo.Instance.RequestVideo(youtubeVideoIdOrUrl,360);
        
    }
}
