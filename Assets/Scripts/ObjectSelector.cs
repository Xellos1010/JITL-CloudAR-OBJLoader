using UnityEngine;
using System.Collections;

public class ObjectSelector : MonoBehaviour {

	public int index =0;
	public GameObject[] object3D;

	// Use this for initialization
	void Start () {
		index = PlayerPrefs.GetInt("myobject3D");
		if (index == null){
			index = 0;
		}
		object3D[index].SetActiveRecursively(true);
	}

	public void SelectNextObj ()
	{
			index = index+1;
			object3D[0].SetActiveRecursively(false);
			object3D[1].SetActiveRecursively(false);
			object3D[2].SetActiveRecursively(false);
			object3D[3].SetActiveRecursively(false);
			object3D[4].SetActiveRecursively(false);
			object3D[index].SetActiveRecursively(true);
	}
	public void ValidateObjSelect()
	{
		PlayerPrefs.SetInt("myobject3D",index);
		// need to add in the variable for the Json object data being sent to VWS
		//
		// grobm
		//
	}

}