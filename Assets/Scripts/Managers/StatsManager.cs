using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum StatName
{
	Range,
	Spread,
	RunSpeed,
	AttackSpeed,
	Strength,
	maxHealth,
	Resistance,
	Knockback,
};

[Serializable]
public class Stat
{
	public StatName statName;
	public int statLevel = 1;
	public float[] levelValue;
}

public class StatsManager : MonoBehaviour 
{

	public Stat[] stats;

	private PlayerInformation playerOneInfo;
	private Health playerHealth;
	private GameObject player;

	// Use this for initialization
	void Start () 
	{

	}

	public float GetStatValue(StatName statName, int level)
	{
		foreach (Stat stat in stats) 
		{
			if (stat.statName == statName)
				return stat.levelValue[level - 1];
		}
		return 0;
	}

	public int GetStatLevel(StatName statName)
	{
		foreach (Stat stat in stats) 
		{
			if (stat.statName == statName)
				return stat.statLevel;
		}
		return 0;
	}

	int GetClosestLevel(StatName statName, float playerStatValue)
	{
		foreach (Stat stat in stats) 
		{
			if (stat.statName == statName) {
				float closestNumber = stat.levelValue [0];
				int valNumber = 0;
				for (int i = 0; i < stat.levelValue.Length; i++) {
					if (Mathf.Abs (stat.levelValue [i] - playerStatValue) < Mathf.Abs (closestNumber - playerStatValue)) {
						closestNumber = stat.levelValue [i];
						valNumber = i;
					}
				}

				return valNumber;
			}
		}
		return 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!player) 
		{
			player = GameObject.FindGameObjectWithTag ("Player1");
		}
		if (player) 
		{
			if (!playerOneInfo) 
			{
				playerOneInfo = player.GetComponent<PlayerInformation> ();
			}
			if (!playerHealth) 
			{
				playerHealth = player.GetComponent<Health> ();
			}
		} 

		//sets the ui to display the correct stat level
		if (player) 
		{
			foreach (Stat stat in stats) 
			{
				if (stat != null) {
					if (stat.statName == StatName.RunSpeed)
						stat.statLevel = GetClosestLevel(StatName.RunSpeed, playerOneInfo.maxMoveSpeed);
					else if (stat.statName == StatName.maxHealth)
						stat.statLevel = GetClosestLevel(StatName.maxHealth, playerHealth.maxHealth);
					else if (stat.statName == StatName.AttackSpeed)
						stat.statLevel = GetClosestLevel(StatName.AttackSpeed, playerOneInfo.attackSpeed);
					else if (stat.statName == StatName.Range)
						stat.statLevel = GetClosestLevel(StatName.Range, playerOneInfo.attackDistance);
					else if (stat.statName == StatName.Resistance)
						stat.statLevel = GetClosestLevel(StatName.Resistance, playerOneInfo.resistance);
					else if (stat.statName == StatName.Spread)
						stat.statLevel = GetClosestLevel(StatName.Spread, playerOneInfo.attackMinAngle);
					else if (stat.statName == StatName.Strength)
						stat.statLevel = GetClosestLevel(StatName.Strength, playerOneInfo.strength);
					else if (stat.statName == StatName.Knockback)
						stat.statLevel = GetClosestLevel(StatName.Knockback, playerOneInfo.knockback);
					
				}
			}
		}
	}
}
