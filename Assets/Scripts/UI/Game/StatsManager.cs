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
	Strength
};

[Serializable]
public class Stat
{
	public StatName statName;
	public Text statText;
	public int statLevel = 1;
	public float[] levelValue;
}

public class StatsManager : MonoBehaviour 
{

	public Stat[] stats;
	private List<PlayerInformation> playersInfo;

	// Use this for initialization
	void Start () 
	{
		playersInfo = new List<PlayerInformation> ();
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		if (players.Length > 0) 
		{
			foreach (GameObject player in players) 
			{
				if (player != null) 
				{
					playersInfo.Add (player.GetComponent<PlayerInformation>());
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//sets the ui to display the correct stat level
		foreach (Stat stat in stats) 
		{
			if (stat != null) 
			{
				stat.statText.text = "" + stat.statName.ToString () + ": " + (stat.statLevel);
			}
		}
		foreach (PlayerInformation playerInfo in playersInfo) 
		{
			foreach (Stat stat in stats) 
			{
				if (stat.statName == StatName.Spread)
					playerInfo.attackMinAngle = stat.levelValue [stat.statLevel - 1];
				else if (stat.statName == StatName.RunSpeed)
					playerInfo.moveSpeed = stat.levelValue [stat.statLevel - 1];
				else if (stat.statName == StatName.Range)
					playerInfo.attackDistance = stat.levelValue [stat.statLevel - 1];
				else if (stat.statName == StatName.Strength)
					playerInfo.strength = stat.levelValue [stat.statLevel - 1];
				else if (stat.statName == StatName.AttackSpeed)
					playerInfo.attackSpeed = stat.levelValue [stat.statLevel - 1];
			}
		}
	}
}
