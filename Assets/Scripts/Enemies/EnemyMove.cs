using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypesOfMoving
{
	BasicFollow,
	FreeRome,
	FollowInRadius, 
	FollowPattern,
	FollowWithIntervals,

};

public class EnemyMove : MonoBehaviour 
{
	public TypesOfMoving movingType;

	//basic move vars
	public float moveSpeed;
	public float lookAtSpeed;

	//follow with intervals vars
	[HideInInspector]
	public float timeTillInterval;
	[HideInInspector]
	public float interval;

	//radius move vars
	[HideInInspector]
	public float radius;

	//free roam vars
	[HideInInspector]
	public float minAdjTime;
	[HideInInspector]
	public float maxAdjTime;

	private GameObject player;

	private float intervalCounter = 0;
	private float timeTillIntervalCounter = 0;
	private float freeRoamAdjCounter = 0;
	private float freeRoamNextAdj = 0;

	private Vector3 wayPoint;

	void FixedUpdate()
	{
		if (player != null) 
		{
			//checks what enum is selected, do that
			if (movingType == TypesOfMoving.BasicFollow) 
			{
				BasicFollow ();
			}
			else if (movingType == TypesOfMoving.FollowInRadius) 
			{
				FollowInRadius ();
			}
			else if (movingType == TypesOfMoving.FollowWithIntervals) 
			{
				FollowWithIntervals ();
			}
			else if (movingType == TypesOfMoving.FreeRome) 
			{
				FreeRoam ();
			}
		} 
		else 
		{
			//get the player if it hasnt
			player = GameObject.FindGameObjectWithTag ("Player");
		}
	}

	void LookAtPlayer()
	{
		//get the direction to the player
		Vector3 direction = player.transform.position - transform.position;
		//get the quaternion using the direction
		Quaternion toRotation = Quaternion.LookRotation (direction);
		//rotate enemy to look at player
		transform.rotation = Quaternion.Slerp (transform.rotation, toRotation, lookAtSpeed * Time.deltaTime);
	}

	void BasicFollow()
	{
		//move towards the player
		transform.position = Vector3.MoveTowards (transform.position, player.transform.position, moveSpeed * Time.deltaTime);
		LookAtPlayer ();
	}

	void FollowInRadius()
	{
		//if the player is less then the radius away from the enemy, follow him
		float distance = Vector3.Distance (transform.position, player.transform.position);
		Debug.Log (distance);
		if (distance < radius) 
		{
			BasicFollow ();
		}
	}

	void FollowWithIntervals()
	{
		bool follow = true;
		timeTillIntervalCounter++;
		//once this float is greater then time till interval, the interval has started
		if (timeTillIntervalCounter > (timeTillInterval * 60)) 
		{
			follow = false;
			intervalCounter++;
			//once this float is greater then the interval, the interval has ended
			if (intervalCounter > (interval * 60)) 
			{
				follow = true;
				intervalCounter = 0;
				timeTillIntervalCounter = 0;
			}
		} 
		if (follow) 
		{
			BasicFollow ();
		}
	}


	void FreeRoam()
	{
		freeRoamAdjCounter++;
		// look at the angle set
		transform.LookAt (wayPoint);
		//go forward!
		transform.position += transform.TransformDirection (Vector3.forward) * moveSpeed * Time.deltaTime;
		//once the counter is greater then the next adjustment time, adjust
		if (freeRoamAdjCounter > freeRoamNextAdj) 
		{
			freeRoamNextAdj = Random.Range ((minAdjTime * 60), (maxAdjTime * 60));
			wayPoint = Random.insideUnitSphere * 47;
			wayPoint.y = 1;
			freeRoamAdjCounter = 0;
		}

	}
}
