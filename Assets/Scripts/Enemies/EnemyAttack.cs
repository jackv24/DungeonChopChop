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
	public int projAmount;

	//interval vars
	[HideInInspector]
	public float timeTillInterval;

	private float shootIntervalCounter = 0;
	private float intervalCounter = 0;
	private int circleAngle = 0;
	private float angle = 0;

	void FixedUpdate()
	{
		circleAngle = 360 / projAmount;
		if (attackingType == TypesOfAttack.BasicShootIntervals) 
		{
			BasicShootIntervals ();
		}
		else if (attackingType == TypesOfAttack.ShootCircleIntervals) 
		{
			ShootCircleIntervals ();
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

	void ShootCircle()
	{
		angle = circleAngle;
		for (int i = 0; i < projAmount; i++)
		{
			//create the projecticle
			GameObject projectile = (GameObject)Instantiate (projecticle.gameObject, transform.position, transform.rotation);
			projectile.transform.Rotate (0, angle, 0);
			Rigidbody rb = projectile.GetComponent<Rigidbody> ();
			//make the projectile move
			rb.AddForce (projectile.transform.forward * thrust, ForceMode.Impulse); 
			angle += circleAngle;
		}
	}

	void ShootCircleIntervals()
	{
		shootIntervalCounter++;
		//count up until the counter has reached the interval time, then shoot bullet and restart
		if (shootIntervalCounter > (timeTillInterval * 60)) 
		{
			ShootCircle ();
			shootIntervalCounter = 0;
		} 
	}

	void ShootCircleRandom()
	{

	}

}
