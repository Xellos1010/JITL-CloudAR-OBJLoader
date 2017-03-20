using UnityEngine;
using System.Collections;

public class UploadImageTargetManager : GameManager {

    public void RetakePhoto()
    {
        SceneManager.LoadIngameScene("TakePictureMode");
    }   
}
