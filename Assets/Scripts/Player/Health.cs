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

	private bool isPoisoned = false;

	private int poisonCounter = 0;
	private float poisonDuration = 0;

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

	void Update()
	{
		if (health <= 0) 
		{
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
	}

	/// <summary>
	/// Sets the poison.
	/// </summary>
	/// <param name="duration">Duration in seconds.</param>
	/// <param name="timeBetweenPoison">Time between poison in seconds.</param>
	public void SetPoison(int damagePerTick, float duration, float timeBetweenPoison)
	{
		poisonDuration = duration;
		isPoisoned = true;
		StartCoroutine (doPoison (damagePerTick, timeBetweenPoison));
	}

	IEnumerator doPoison(int damagePerTick, float timeBetweenPoison)
	{
		while (isPoisoned) 
		{
			yield return new WaitForSeconds (timeBetweenPoison);
			AffectHealth (-damagePerTick);
		}
	}
}
