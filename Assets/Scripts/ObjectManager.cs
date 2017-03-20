using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {

    public int currentActiveObject;
    GameObject currentActiveGameObject;
    public GameObject[] augmentableObjects;

    public void Awake()
    {
        InitializeAugmentableObjects();
        ToggleObjects(false);
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("UploadImageVuphoria").IsValid())
            ToggleObject(0, true);
    }

    void InitializeAugmentableObjects()
    {
        GameObject[] augmentablePrefabs = Resources.LoadAll<GameObject>("AugmentableObjects");
        Debug.Log("augmentablePrefabs.Length = " + augmentablePrefabs.Length);
        augmentableObjects = new GameObject[augmentablePrefabs.Length+transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            augmentableObjects[i] = child.gameObject;
            i++;
        }
        int iTotalChildCountBeforeInstantiate = transform.childCount;
        Debug.Log("iTotalChildCountBeforeInstantiate =" + iTotalChildCountBeforeInstantiate);
        for (i = iTotalChildCountBeforeInstantiate; i < augmentableObjects.Length ; i++)
        {
            try {
                augmentableObjects[i] = (GameObject)Instantiate(augmentablePrefabs[i - iTotalChildCountBeforeInstantiate],
                    transform.position,
                    transform.rotation, transform);
                augmentableObjects[i].name = augmentableObjects[i].name.Remove(augmentableObjects[i].name.Length - 7);
                Debug.Log("augmentablePrefabs[i] = " + augmentablePrefabs[i - iTotalChildCountBeforeInstantiate].ToString());
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message + " the array is at " + i + " total total number of objects are " + (augmentablePrefabs.Length + transform.childCount));
            }
        }
    }

    public void DeactivateActiveObject()
    {
        ToggleObject(currentActiveObject, false);
    }

    public void ToggleNextObject()
    {
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
                break;
            }
        DeactivateActiveObject();
        
        if (currentActiveObject+1 >= transform.childCount)
            currentActiveObject = 0;
        else
            currentActiveObject += 1;
        ToggleObject(currentActiveObject, true);
    }

    public void TogglePreviousObject()
    {
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
                break;
            }
        DeactivateActiveObject();
        if (currentActiveObject - 1 < 0)
        {
            currentActiveObject = transform.childCount - 1;
        }
        else
            currentActiveObject -= 1;
        ToggleObject(currentActiveObject, true);
    }

    public void ToggleNextObjectWithWait()
    {
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
                break;
            }
        DeactivateActiveObject();
        if (currentActiveObject+1 >= transform.childCount)
            currentActiveObject = 0;
        else
            currentActiveObject += 1;
        ToggleObjectWithWait(currentActiveObject, true);
    }

    public void TogglePreviousObjectWithWait()
    {
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
                break;
            }
        DeactivateActiveObject();
        if (currentActiveObject - 1 >= 0)
        {
            currentActiveObject = transform.childCount - 1;
        }
        else
        {
            currentActiveObject -= 1;
        }
        ToggleObjectWithWait(currentActiveObject, true);
    }

    public void ToggleObjects(bool tf)
    {
        foreach (GameObject augmentObject in augmentableObjects)
        {
            try {
                augmentObject.SetActive(tf);
                ToggleMeshRenderer(augmentObject, tf);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    void ToggleMeshRenderer(GameObject objectToToggle, bool tf)
    {
        if (objectToToggle.GetComponent<MeshRenderer>())
            objectToToggle.GetComponent<MeshRenderer>().enabled = tf;
        if (objectToToggle.GetComponent<SkinnedMeshRenderer>())
            objectToToggle.GetComponent<SkinnedMeshRenderer>().enabled = tf;
        foreach (Transform child in objectToToggle.transform)
        {
            ToggleMeshRenderer(child.gameObject, tf);
        }
    }

    public void ToggleObject(int augmentedObject, bool tf)
    {        
        GameObject augmentableObject = transform.GetChild(augmentedObject).gameObject;
        if(tf)
            currentActiveObject = augmentedObject;
        ToggleObject(augmentableObject, tf);
        
    }

    public void ToggleObject(GameObject augmentedObject, bool tf)
    {
        Debug.Log("Toggling " + augmentedObject.ToString() + " to " + tf.ToString());
        try
        {
            if (tf)
            {
                Debug.Log("Object selected = " + ((augmentedObject.name.Contains("Clone")) ? augmentedObject.name.Remove(augmentedObject.name.Length - 7) : augmentedObject.name));
                VWSCloudConnecter.instance.SetActiveObjectName((augmentedObject.name.Contains("Clone")) ? augmentedObject.name.Remove(augmentedObject.name.Length - 7) : augmentedObject.name);                
            }
        }
        catch
        {
            Debug.Log("No VWSCloud Connector Instance");
        }
        augmentedObject.SetActive(tf);
        ToggleMeshRenderer(augmentedObject,tf);
        if (augmentedObject.GetComponent<CycleThroughAnimations>())
            augmentedObject.GetComponent<CycleThroughAnimations>().Initialize();
    }

    public void ToggleObjectWithWait(int augmentedObject, bool tf)
    {
        Debug.Log("Toggling " + augmentedObject.ToString() + " to " + tf.ToString());
        GameObject augmentableObject = transform.GetChild(augmentedObject).gameObject;
        if (tf)
            currentActiveObject = augmentedObject;
        StartCoroutine(WaitAndSetActive(augmentableObject, .25f, tf));
        Debug.Log("Set " + augmentableObject.ToString() + " Active to " + tf.ToString());
    }

    public void ToggleObjectWithWait(GameObject augmentedObject, bool tf)
    {
        Debug.Log("Toggling " + augmentedObject.ToString() + " to " + tf.ToString());
        StartCoroutine(WaitAndSetActive(augmentedObject, .25f,tf));
    }

    IEnumerator WaitAndSetActive(GameObject objectToActivate, float time, bool onOff)
    {
        Debug.Log("Waiting to activate " + objectToActivate.name);
        yield return new WaitForSeconds(time);
        ToggleObject(objectToActivate, onOff);
        Debug.Log("Activated " + objectToActivate.name);
    }
}