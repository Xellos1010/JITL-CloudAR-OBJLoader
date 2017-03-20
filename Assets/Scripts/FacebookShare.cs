using UnityEngine;
using Facebook.Unity;
using System;
using System.Collections.Generic;

public static class FacebookShare {

    public static Func<bool> initialized;
    public static Func<bool> loggedIn;
    // Use this for initialization
	public static void FBInit()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
            initialized();
        }
    }

    public static void LogIn()
    {
        List<string> perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);        
    }

    public static void Share(string imageURL)
    {
        if (FB.IsLoggedIn)
        {
            FB.ShareLink(
          new Uri("https://play.google.com/store/apps/details?id=com.care4d.aow"),
          contentTitle: "I've made my mARx!!!",
          contentDescription: "Check out the mARx I've left.",
          photoURL: new Uri(imageURL),
          callback: ShareCallback
          );
        }
        else
        {
            Debug.Log("User not Logged In");
        }
    }

    private static void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            initialized();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private static void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private static void AuthCallback(ILoginResult result)    {
        if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log(aToken.UserId);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions)
                {
                    Debug.Log(perm);
                }
                loggedIn();
            }
            else
            {
                Debug.Log("User cancelled login");                
            }
        }

    private static void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }
}
