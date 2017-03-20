using UnityEngine;
using System;
using System.Collections;

public class ServerAccessFunctions : MonoBehaviour
{

    private static ServerAccessFunctions _instance;
    public static ServerAccessFunctions instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<ServerAccessFunctions>();
            return _instance;
        }

    }

    IEnumerator CheckDeviceRegistered(string deviceUUID)
    {
        yield return StartCoroutine(ServerAccessCalls.GetUserName(SystemInfo.deviceUniqueIdentifier));
    }

    IEnumerator SetupUserInformation()
    {
        if (ServerAccessCalls.Username == "")
        {
            Debug.Log("Getting Username");
            yield return StartCoroutine(ServerAccessCalls.GetUserName(SystemInfo.deviceUniqueIdentifier));
        }
    }
}



