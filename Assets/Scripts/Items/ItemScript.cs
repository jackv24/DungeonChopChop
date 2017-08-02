using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

	public float destroyTime = 20;

	void OnEnable()
	{
		StartCoroutine (waitToHide ());
	}
		
	IEnumerator waitToHide()
	{
		yield return new WaitForSeconds (destroyTime);
		gameObject.SetActive (false);
	}

}
