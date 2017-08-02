using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfDeath
{
	Nothing,
	SplitsIntoAnotherEnemy,
	CircleExplode,
};

public class EnemyDeath : MonoBehaviour 
{

	public TypesOfDeath deathType;

	//splits into enemy vals
	[HideInInspector]
	public int amountToSplit;
	[HideInInspector]
	public GameObject splitEnemy;

	private Health health;
	private bool dead = false;

	// Use this for initialization
	void Start () 
	{
		health = GetComponent<Health> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (health.isDead) 
		{
			if (!dead) 
			{
				//checks to make sure if dead
				dead = true;
				ChoseDeath ();
			}
		}
		if (health.health <= 0) {
			ChoseDeath ();
		}
	}

	void ChoseDeath()
	{
		if (deathType == TypesOfDeath.SplitsIntoAnotherEnemy) 
		{
			SplitEnemy ();
		} 
		else if (deathType == TypesOfDeath.Nothing) 
		{
			Die ();
		}
	}

	void SplitEnemy()
	{
		if (splitEnemy != null)
		{
			for (int i = 0; i < amountToSplit; i++) 
			{
				//create the split enemies and set them to this position
				GameObject enemy = ObjectPooler.GetPooledObject (splitEnemy);
				enemy.transform.position = transform.position;
				Die ();
			}
		}
	}

	void Die()
	{
		//do die particles and stuff
		gameObject.SetActive(false);
	}

}
