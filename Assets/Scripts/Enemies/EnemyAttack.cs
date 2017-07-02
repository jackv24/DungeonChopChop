using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfAttack
{
	BasicShootIntervals,
	BasicShootRandom,
	ShootCircleIntervals,
	ShootCircleRandom,
};

public class EnemyAttack : MonoBehaviour
{

	public TypesOfAttack attackingType;
	public ProjecticleVariables projecticle;

	public float thrust;

	//shoot circle vars
	[HideInInspector]
	public float radius;

	//Interval vars
	[HideInInspector]
	public float timeTillInterval;

	private float shootIntervalCounter = 0;
	private float intervalCounter = 0;

	void FixedUpdate()
	{
		if (attackingType == TypesOfAttack.BasicShootIntervals) 
		{
			BasicShootIntervals ();
		}
	}

	void Shootforward()
	{
		//create the projecticle
		GameObject projectile = (GameObject)Instantiate (projecticle.gameObject, transform.position, transform.rotation);
		Rigidbody rb = projectile.GetComponent<Rigidbody> ();
		//make the projectile move
		rb.AddForce (transform.forward * thrust, ForceMode.Impulse); 
		Destroy (projectile, 3);
	}

	void BasicShootIntervals()
	{
		shootIntervalCounter++;
		//count up until the counter has reached the interval time, then shoot bullet and restart
		if (shootIntervalCounter > (timeTillInterval * 60)) 
		{
			Shootforward ();
			shootIntervalCounter = 0;
		} 
	}

	void BasicShootRandom()
	{

	}

	void ShootCircleIntervals()
	{

	}

	void ShootCircleRandom()
	{

	}

}
