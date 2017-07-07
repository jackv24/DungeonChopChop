using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfAttack
{
	BasicShootIntervals,
	BasicShootRandIntervals,
	ShootCircleIntervals,
	ShootCircleRandIntervals,
};

public class EnemyAttack : MonoBehaviour
{

	public TypesOfAttack attackingType;
	public ProjecticleVariables projecticle;

	public float thrust;
	[Tooltip("This values x projectile damage")]
	public float attackStrength;

	//shoot circle vars
	[HideInInspector]
	public int projAmount;

	//interval vars
	[HideInInspector]
	public float timeTillInterval;

	//shoot random vars
	[HideInInspector]
	public float minInterval;
	[HideInInspector]
	public float maxInterval;

	private float shootIntervalCounter = 0;
	private int circleAngle = 0;
	private float angle = 0;
	private float randomInterval = 0;

	void FixedUpdate()
	{
		circleAngle = 360 / projAmount;
		randomInterval = Random.Range (minInterval, maxInterval);
		if (attackingType == TypesOfAttack.BasicShootIntervals) 
		{
			BasicShootIntervals ();
		}
		else if (attackingType == TypesOfAttack.ShootCircleIntervals) 
		{
			ShootCircleIntervals ();
		}
		else if (attackingType == TypesOfAttack.ShootCircleRandIntervals) 
		{
			ShootCircleRandIntervals ();
		}
		else if (attackingType == TypesOfAttack.BasicShootRandIntervals) 
		{
			BasicShootRandIntervals ();
		}
	}

	void Shootforward()
	{
		//create the projecticle
		GameObject projectile = ObjectPooler.GetPooledObject(projecticle.gameObject);
		projectile.transform.position = transform.position;
		projectile.transform.rotation = transform.rotation;;
		Rigidbody rb = projectile.GetComponent<Rigidbody> ();
		projectile.GetComponent<ProjectileCollision> ().damageMultiplyer = attackStrength;
		//make the projectile move
		rb.AddForce (transform.forward * thrust, ForceMode.Impulse); 
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

	void BasicShootRandIntervals()
	{
		shootIntervalCounter++;
		//count up until the counter has reached the interval time, then shoot bullet and restart
		if (shootIntervalCounter > (randomInterval * 60)) 
		{
			randomInterval = Random.Range (minInterval, maxInterval);
			Shootforward ();
			shootIntervalCounter = 0;
		} 
	}

	void ShootCircle()
	{
		angle = circleAngle;
		for (int i = 0; i < projAmount; i++)
		{
			//create the projecticle
			GameObject projectile = ObjectPooler.GetPooledObject(projecticle.gameObject);
			projectile.transform.position = transform.position;
			projectile.transform.rotation = transform.rotation;
			projectile.transform.Rotate (0, angle, 0);
			Rigidbody rb = projectile.GetComponent<Rigidbody> ();
			projectile.GetComponent<ProjectileCollision> ().damageMultiplyer = attackStrength;
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

	void ShootCircleRandIntervals()
	{
		shootIntervalCounter++;
		//count up until the counter has reached the interval time, then shoot bullet and restart
		if (shootIntervalCounter > (randomInterval * 60)) 
		{
			randomInterval = Random.Range (minInterval, maxInterval);
			ShootCircle ();
			shootIntervalCounter = 0;
		} 
	}

}
