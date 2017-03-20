using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ServerAccessCalls
{
    private static string secretKey = "1337"; // Edit this value and make sure it's the same as the one stored on the server
    
    public static string GetUsernameURL = "http://augmentourworld.com/care4d/GetUsername.php?";
    public static string GetSubscriptionInfoURL = "http://augmentourworld.com/care4d/GetDeviceIDSubscriptions.php?";
    public static string CheckUserLoginURL = "http://augmentourworld.com/care4d/CheckLoginDetails.php?";
    public static string RegisterDeviceURL = "http://augmentourworld.com/care4d/RegisterDevice.php?";
    public static string CheckDeviceRegisteredURL = "http://augmentourworld.org/care4d/CheckDeviceRegistered.php?";    
    public static string Username = "";
    public static string selectedGradeLevel = "";
    public static bool deviceRegistered = false;
   

    public static IEnumerator GetUserName(string DeviceID)
    {
        string post_url = GetUsernameURL + "uuid=" + DeviceID;
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        Debug.Log("Details from Server " + hs_post.text);
        if (hs_post.text.ToLower().Contains("null"))
        {
            Username = "";
            Debug.Log("Subscription Number from server is null - returning false");
            yield return false;
        }
        else
        {
            Username = hs_post.text;
            yield return true;            
        }
    }

    public static IEnumerator CheckDeviceRegistered(string uuid)
    {
        string post_url = CheckDeviceRegisteredURL + "uuid=" + WWW.EscapeURL(uuid);
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        Debug.Log("Device Registered data from server is " + hs_post.text);
        if (hs_post.error != null)
        {
            Debug.Log("There was an error getting Grade Info: " + hs_post.error);
            //yield return false;
        }
        else if (hs_post.text.ToLower().Contains("null"))
        {
            Debug.Log("There was an error getting subscription Info: " + hs_post.text);
            yield return false;
        }
        if (hs_post.text.ToLower() == "true")
        {
            deviceRegistered = true;
        }
        Debug.Log(hs_post.text);
        //yield return hs_post.text;
    }

    /*public static IEnumerator Register(string Name, string DeviceID)
    {
        string post_url = SubmitUsernameURL + "d=" + DeviceID  + "&u=" + Name;
        Debug.Log(post_url);
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        Debug.Log(hs_post.text);
        if (hs_post.error != null)
        {
            Debug.Log("There was an error posting the high score: " + hs_post.error);
        }
        else
        {
            Debug.Log("Success");
            Username = Name;
            GetUserName(SystemInfo.deviceUniqueIdentifier);
        }
    }*/

    /*public static IEnumerator UpdateUsername(string userName, string device_id)
    {
        string post_url = UpdateUsernameURL + "u=" + WWW.EscapeURL(userName) + "&d=" + WWW.EscapeURL(device_id.ToString());
        Debug.Log(post_url);
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        if (hs_post.error != null)
        {
            Debug.Log("There was an error posting the high score: " + hs_post.error);
            yield return false;
        }
        else
        {
            Debug.Log("Success");
            yield return true;
        }
    }   
    */
    public static IEnumerator CheckUserDetails(string username, string password)
    {
        string post_url = CheckUserLoginURL + "u=" + WWW.EscapeURL(username) + "&p=" + WWW.EscapeURL(password);
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        if (hs_post.error != null)
        {
            Debug.Log("There was an error checking User Details: " + hs_post.error);
        }
        Username = username;
        yield return hs_post.text;
    }

    public static IEnumerator GetGradeInfo()
    {
        if (Username.Length > 0)
        {
            string post_url = GetSubscriptionInfoURL + "user=" + WWW.EscapeURL(Username);
            WWW hs_post = new WWW(post_url);
            yield return hs_post; // Wait until the download is done
            if (hs_post.error != null)
            {
                Debug.Log("There was an error getting Grade Info: " + hs_post.error);
            }
            Debug.Log("GetGradeInfo returned " + hs_post.text);
            yield return hs_post.text;
        }
        else
            yield return null;
    }

    public static IEnumerator GetGradeInfo(string DeviceID)
    {
        string post_url = GetSubscriptionInfoURL + "uuid=" + WWW.EscapeURL(DeviceID);
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        if (hs_post.error != null)
        {
            Debug.Log("There was an error getting Grade Info: " + hs_post.error);
        }
        yield return hs_post.text;
    }

    public static IEnumerator RegisterDevice(string DeviceID, string Username)
    {
        string post_url = RegisterDeviceURL + "uuid=" + WWW.EscapeURL(DeviceID)+"&u="+ WWW.EscapeURL(Username);
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        if (hs_post.error != null)
        {
            Debug.Log("There was an error registering Info: " + hs_post.error);
            yield return false;
        }
        else if(hs_post.text.Contains("null"))
        {
            Debug.Log("There was an error registering Info: " + hs_post.text);
            yield return false;
        }
        deviceRegistered = true;
        yield return true;
    }

    public static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}

