using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour 
{
	[Header("Player Values")]
	public int playerIndex = 0;
	public float attackMinAngle = 130;
	public float attackDistance = 5;
	public float dashDistance = 5;
	public float moveSpeed;
	public float strength;
	public float attackSpeed;
	public float resistance;

	[Header("Stat Levels")]
	public int attackSpreadLevel = 1;
	public int attackDistanceLevel = 1;
	public int moveSpeedLevel = 1;
	public int strengthLevel = 1;
	public int attackSpeedLevel = 1;
	public int resistanceLevel = 1;
	public int maxHealthLevel = 1;

	[Header("Charm")]
	public Charms currentCharm;

	private StatsManager statsManager;
	private Health health;

	private WeaponStats currentWeaponStats;

	void Start()
	{
		health = GetComponent<Health> ();

		GameObject s = GameObject.FindGameObjectWithTag("StatsManager");
		if(s)
			statsManager = s.GetComponent<StatsManager> ();
	}

	void Update()
	{
		//set stat values depending on level
		if (statsManager) 
		{
			attackMinAngle = statsManager.GetStatValue (StatName.AttackSpeed, attackSpreadLevel); 
			attackDistance = statsManager.GetStatValue (StatName.Range, attackDistanceLevel);
			moveSpeed = statsManager.GetStatValue (StatName.RunSpeed, moveSpeedLevel); 
			strength = statsManager.GetStatValue (StatName.Strength, strengthLevel);
			attackSpeed = statsManager.GetStatValue (StatName.AttackSpeed, attackSpeedLevel);
			resistance = statsManager.GetStatValue (StatName.Resistance, resistanceLevel);
			health.maxHealth = (int)statsManager.GetStatValue (StatName.maxHealth, maxHealthLevel); 
		}
		//sets stats depending on weapon values
		if (currentWeaponStats) 
		{
			attackSpreadLevel = currentWeaponStats.spreadLevel;
			attackSpeedLevel = currentWeaponStats.speedLevel;
			attackDistanceLevel = currentWeaponStats.distanceLevel;
		} 
		else 
		{
			currentWeaponStats = GetComponentInChildren<WeaponStats> ();
		}
	}
}
