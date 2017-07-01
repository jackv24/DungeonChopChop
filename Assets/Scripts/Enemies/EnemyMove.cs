using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypesOfMoving
{
	BasicFollow,
	FreeRome,
	RandomRome,
	FollowInRadius, 
	FollowPattern,
	FollowWithIntervals,

};

public class EnemyMove : MonoBehaviour 
{
	public TypesOfMoving movingType;

	//basic move vars
	public float moveSpeed;

	//follow with intervals
	[HideInInspector]
	public float timeTillInterval;
	[HideInInspector]
	public float interval;

	//radius move vars
	[HideInInspector]
	public float radius;

	private GameObject player;

	private float intervalCounter = 0;
	private float timeTillIntervalCounter = 0;

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
		} 
		else 
		{
			//get the player if it hasnt
			player = GameObject.FindGameObjectWithTag ("Player");
		}
	}

	void BasicFollow()
	{
		//move towards the player
		transform.position = Vector3.MoveTowards (transform.position, player.transform.position, moveSpeed * Time.deltaTime);
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
}
