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
	
	// Update is called once per frame
	void Update ()
	{
		
		if (!player) 
		{
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		if (player) 
		{
			if (!playerOneInfo) 
			{
				playerOneInfo = player.GetComponent<PlayerInformation> ();
			}
		} 

		//sets the ui to display the correct stat level
		if (player) 
		{
			foreach (Stat stat in stats) 
			{
				if (stat != null) {
					if (stat.statName == StatName.AttackSpeed)
						stat.statLevel = playerOneInfo.attackSpeedLevel;
					else if (stat.statName == StatName.maxHealth)
						stat.statLevel = playerOneInfo.maxHealthLevel;
					else if (stat.statName == StatName.Range)
						stat.statLevel = playerOneInfo.attackDistanceLevel;
					else if (stat.statName == StatName.Resistance)
						stat.statLevel = playerOneInfo.resistanceLevel;
					else if (stat.statName == StatName.RunSpeed)
						stat.statLevel = playerOneInfo.moveSpeedLevel;
					else if (stat.statName == StatName.Spread)
						stat.statLevel = playerOneInfo.attackSpreadLevel;
					else if (stat.statName == StatName.Strength)
						stat.statLevel = playerOneInfo.strengthLevel;
					else if (stat.statName == StatName.Knockback)
						stat.statLevel = playerOneInfo.knockbackLevel;
					
				}
			}
		}
	}
}
