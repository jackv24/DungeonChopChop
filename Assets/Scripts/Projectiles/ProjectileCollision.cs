using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour {

	public int damageAmount;

	[HideInInspector]
	public float damageMultiplyer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.GetComponent<Health>())
		{
			col.transform.GetComponent<Health> ().AffectHealth ((int)(-damageAmount * damageMultiplyer));
			//check to see if collider has an animator
			if (col.gameObject.GetComponentInChildren<Animator> ())
			{
				col.gameObject.GetComponentInChildren<Animator> ().SetTrigger ("Hit");
			}
			gameObject.SetActive (false);
		}
	}

}
