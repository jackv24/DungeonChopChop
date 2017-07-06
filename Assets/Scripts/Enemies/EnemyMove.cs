using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypesOfMoving
{
	Follow,
	Roam,
	Static,
};

public enum MoveTimes
{
	Constant,
	Interval,
	Stutter,
	Charge,
	Hop,
};

public enum MoveDistances
{
	NoDistance,
	Radius,
	InSight,
};

public class EnemyMove : MonoBehaviour 
{
	[Header("Basic Movement Values")]
	//basic move vars
	public LayerMask groundMask;
	public float moveSpeed;
	public float lookAtSpeed;

	public TypesOfMoving movingType;
	public MoveTimes moveTimes;
	public MoveDistances moveDistances;

	[Header("Move Choice Values")]

	//follow with intervals vars
	[HideInInspector]
	public float timeTillInterval;
	[HideInInspector]
	public float interval;

	//stutter vals
	[HideInInspector]
	public float power;
	[HideInInspector]
	public float timeBetweenStutter;

	//charge vals
	[HideInInspector]
	public float chargeUptime;

	//radius move vars
	[HideInInspector]
	public float radius;

	//hop vars
	[HideInInspector]
	public float jumpPower;

	//free roam vars
	[HideInInspector]
	public float minAdjTime;
	[HideInInspector]
	public float maxAdjTime;

	//inSight vars
	[HideInInspector]
	public float distance;

	private GameObject player;
	private Rigidbody rb;

	private float intervalCounter = 0;
	private float timeTillIntervalCounter = 0;
	private float freeRoamAdjCounter = 0;
	private float freeRoamNextAdj = 0;

	private bool isGrounded = false;
	private Vector3 wayPoint;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
	}

	void OnDrawGizmos()
	{
		Debug.DrawRay (new Vector3 (transform.position.x, (transform.position.y - transform.lossyScale.y + .3f), transform.position.z), -transform.up);
	}

	void Update()
	{
		if (Physics.Raycast (new Vector3(transform.position.x, (transform.position.y - transform.localScale.y + .3f), transform.position.z) , -transform.up, .4f, groundMask)) 
		{
			isGrounded = true;
		} 
		else 
		{
			isGrounded = false;
		}
	}

	void FixedUpdate()
	{
		if (player != null) {
			switch (movingType) {
			//check what type of moving
			//------------------------------------------------Follow
			case TypesOfMoving.Follow:
				//check what time of time the enemy moves
				switch (moveTimes) {
				case MoveTimes.Constant:
					//if the enemy moves within a specific distance
					switch (moveDistances) {
					case MoveDistances.InSight:
						FollowConstantInSight ();
						break;
					case MoveDistances.NoDistance:
						BasicFollow ();
						break;
					case MoveDistances.Radius:
						FollowConstantRadius ();
						break;
					}
					break;
				//check what time of time the enemy moves
				case MoveTimes.Interval:
					//if the enemy moves within a specific distance
					switch (moveDistances) {
					case MoveDistances.InSight:
						FollowIntervalInSight ();
						break;
					case MoveDistances.NoDistance:
						FollowIntervalNoDistance ();
						break;
					case MoveDistances.Radius:
						FollowIntervalRadius ();
						break;
					}
					break;
				//check what time of time the enemy moves
				case MoveTimes.Stutter:
					//if the enemy moves within a specific distance
					switch (moveDistances) {
					case MoveDistances.InSight:
						break;
					case MoveDistances.NoDistance:
						FollowStutter ();
						break;
					case MoveDistances.Radius:
						break;
					}
					break;
				//check what time of time the enemy moves
				case MoveTimes.Hop:
					//if the enemy moves within a specific distance
					switch (moveDistances) {
					case MoveDistances.InSight:
						break;
					case MoveDistances.NoDistance:
						Hop ();
						break;
					case MoveDistances.Radius:
						break;
					}
					break;
				}
				break;
			//check what type of moving
			//------------------------------------------------Roam
			case TypesOfMoving.Roam:
				//check what time of time the enemy moves
				switch (moveTimes) {
				case MoveTimes.Constant:
					//if the enemy moves within a specific distance
					switch (moveDistances) {
					case MoveDistances.InSight:
						break;
					case MoveDistances.NoDistance:
						Roam ();
						break;
					case MoveDistances.Radius:
						RoamWithRadius ();
						break;
					}
					break;
				//check what time of time the enemy moves
				case MoveTimes.Interval:
					//if the enemy moves within a specific distance
					switch (moveDistances) {
					case MoveDistances.InSight:
						break;
					case MoveDistances.NoDistance:
						RoamIntervalNoDistance ();
						break;
					case MoveDistances.Radius:
						RoamIntervalRadius ();
						break;
					}
					break;
				}
				break;
			//check what type of moving
			//------------------------------------------------Static
			case TypesOfMoving.Static:
				//check what time of time the enemy moves
				switch (moveTimes) {
				case MoveTimes.Charge:
					//if the enemy moves within a specific distance
					switch (moveDistances) {
					case MoveDistances.InSight:
						break;
					case MoveDistances.NoDistance:
						Charge ();
						break;
					case MoveDistances.Radius:
						break;
					}
					break;
				}
				break;
			}
		}
		else 
		{
			//get the player if it hasnt
			player = GameObject.FindGameObjectWithTag ("Player");
		}
	}

	bool InRadiusOfPlayer()
	{
		//if the player is less then the radius away from the enemy, follow him
		float distance = Vector3.Distance (transform.position, player.transform.position);
		if (distance < radius) {
			return true;
		} 
		else 
		{
			return false;
		}
	}

	bool canSeePlayer()
	{
		RaycastHit hit;
		//check to see if the enemy is looking at the player
		if (Physics.Raycast (transform.position, transform.forward, out hit, distance)) 
		{
			//if it sees the player return true, else return false
			if (hit.collider.tag == "Player")
			{
				return true;
			}
			return false;
		}
		return false;
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

	void FollowConstantRadius()
	{
		//if the player is less then the radius away from the enemy, follow him
		if (InRadiusOfPlayer ()) 
		{
			BasicFollow ();
		}
	}

	void FollowConstantInSight()
	{
		if (canSeePlayer()) 
		{
			BasicFollow ();
		}
	}

	void FollowIntervalInSight()
	{
		if (canSeePlayer()) 
		{
			FollowIntervalNoDistance ();
		}
	}

	void FollowIntervalNoDistance()
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

	void FollowIntervalRadius()
	{
		//if the player is less then the radius away from the enemy, follow him
		if (InRadiusOfPlayer ())
		{
			FollowIntervalNoDistance ();
		}
	}

	void Stutter()
	{
		rb.AddForce (transform.forward * power, ForceMode.Impulse);
	}

	void FollowStutter()
	{
		BasicFollow ();
		timeTillIntervalCounter++;
		//once this float is greater then time till interval, the interval has started
		if (timeTillIntervalCounter > (timeBetweenStutter * 60)) 
		{
			Stutter ();
			timeTillIntervalCounter = 0;
		} 
	}


	void Roam()
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

	void RoamWithRadius()
	{
		if (InRadiusOfPlayer ()) 
		{
			Roam ();
		}
	}

	void RoamIntervalRadius()
	{
		if (InRadiusOfPlayer ()) 
		{
			RoamIntervalNoDistance ();
		}
	}

	void RoamIntervalNoDistance()
	{
		bool roam = true;
		timeTillIntervalCounter++;
		//once this float is greater then time till interval, the interval has started
		if (timeTillIntervalCounter > (timeTillInterval * 60)) 
		{
			roam = false;
			intervalCounter++;
			//once this float is greater then the interval, the interval has ended
			if (intervalCounter > (interval * 60)) 
			{
				roam = true;
				intervalCounter = 0;
				timeTillIntervalCounter = 0;
			}
		} 
		if (roam) 
		{
			Roam ();
		}
	}

	void Charge()
	{
		LookAtPlayer ();
		timeTillIntervalCounter++;
		//once this float is greater then time till charge up time, do the stutter
		if (timeTillIntervalCounter > (chargeUptime * 60)) {
			timeTillIntervalCounter = 0;
			Stutter ();
		}
	}

	void Hop()
	{
		BasicFollow ();
		if (isGrounded) {
			rb.AddForce (transform.forward * power, ForceMode.Impulse);
			rb.AddForce (transform.up * jumpPower, ForceMode.Impulse);
		}
	}

}
