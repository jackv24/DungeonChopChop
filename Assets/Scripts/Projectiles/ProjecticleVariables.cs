using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjecticleVariables : MonoBehaviour 
{
	public float hideTime;

	private Rigidbody rb;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		StartCoroutine(waitHide ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void Reset()
	{
		if (rb) 
		{
			rb.velocity = new Vector3 (0, 0, 0);
		}
	}

	void OnEnable()
	{
		Reset ();
		StartCoroutine(waitHide ());
	}

	IEnumerator waitHide()
	{
		yield return new WaitForSeconds (hideTime);
		gameObject.SetActive (false);
	}

}
