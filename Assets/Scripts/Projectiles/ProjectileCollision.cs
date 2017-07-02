using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour {

	public int damageAmount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.tag == "Player")
		{
			col.transform.GetComponent<Health> ().AffectHealth (-damageAmount);
			gameObject.SetActive (false);
		}
	}

}
