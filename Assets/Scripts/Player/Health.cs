using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour 
{

	public float maxHealth;
	public float health;
	public bool isDead = false;

	public delegate void HealthEvent();
	public event HealthEvent OnDeath;
	public event HealthEvent OnHealthChange;

	public bool isPoisoned = false;
	public bool isBurned = false;

	private int poisonCounter = 0;
	private float poisonDuration = 0;

	private PlayerInformation playerInfo;

	public void AffectHealth(float healthDeta)
	{
		health += healthDeta;
		if (OnHealthChange != null) 
		{
			OnHealthChange ();
		}
		if (health > maxHealth) 
		{
			health = maxHealth;
		}
		if (health <= 0 && isDead == false) 
		{
			isDead = true;
			if (OnDeath != null) 
			{
				OnDeath ();
			}
			Death ();
		}
	}

	void Start()
	{
		if (GetComponent<PlayerInformation> ()) 
		{
			playerInfo = GetComponent<PlayerInformation> ();
		}
	}

	void Update()
	{
		if (health <= 0) 
		{
			health = 0;
			isDead = true;
			Death ();
		}
		if (health > maxHealth) 
		{
			health = maxHealth;
		}
	}

	void FixedUpdate()
	{
		if (isPoisoned) 
		{
			poisonCounter++;
			if (poisonCounter >= (poisonDuration * 60)) 
			{
				isPoisoned = false;
			}
		}
	}
		
	public void Death()
	{
		//do death
		//checks if the game has an enemy drop script, if it does it is an enemy
		if (transform.GetComponent<EnemyDrops> ()) 
		{
			transform.GetComponent<EnemyDrops> ().DoDrop ();
		}
	}

	/// <summary>
	/// Sets the poison.
	/// </summary>
	/// <param name="duration">Duration in seconds.</param>
	/// <param name="timeBetweenPoison">Time between poison in seconds.</param>
	public void SetPoison(float damagePerTick, float duration, float timeBetweenPoison)
	{
		if (playerInfo)
			damagePerTick = playerInfo.GetCharmFloat ("poisonMultiplier");
		poisonDuration = duration;
		isPoisoned = true;
		StartCoroutine (doPoison (damagePerTick, duration, timeBetweenPoison));
	}

	IEnumerator doPoison(float damagePerTick, float duration, float timeBetweenPoison)
	{
		int counter = 0;
		while (isPoisoned) 
		{
			counter++;
			yield return new WaitForSeconds (timeBetweenPoison);
			AffectHealth (-damagePerTick);
			if (counter >= duration) 
			{
				isPoisoned = false;
			}
		}
	}

	/// <summary>
	/// Sets the burn.
	/// </summary>
	/// <param name="duration">Duration in seconds.</param>
	/// <param name="timeBetweenBurn">Time between poison in seconds.</param>
	public void SetBurned(float damagePerTick, float duration, float timeBetweenBurn)
	{
		if (playerInfo)
			damagePerTick = playerInfo.GetCharmFloat ("burnMultiplier");
		poisonDuration = duration;
		isBurned = true;
		StartCoroutine (doBurn (damagePerTick, duration, timeBetweenBurn));
	}

	IEnumerator doBurn(float damagePerTick, float duration, float timeBetweenBurn)
	{
		int counter = 0;
		while (isBurned) 
		{
			counter++;
			yield return new WaitForSeconds (timeBetweenBurn);
			AffectHealth (-damagePerTick);
			if (counter >= duration) 
			{
				isBurned = false;
			}
		}
	}
}
