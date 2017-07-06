﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour 
{

	public int maxHealth;
	public int health;
	public bool isDead = false;

	public delegate void HealthEvent();
	public event HealthEvent OnDeath;
	public event HealthEvent OnHealthChange;

	public void AffectHealth(int healthDeta)
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

	public void Death()
	{
		//do death
	}
}
