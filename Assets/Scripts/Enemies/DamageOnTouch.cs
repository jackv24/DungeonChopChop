using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour 
{

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
			col.gameObject.GetComponent<Health> ().AffectHealth (-damageAmount);
		}
	}

}
