using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour 
{

	public int playerIndex = 0;
	public float attackMinAngle = 130;
	public float attackDistance = 5;
	public float moveSpeed;
	public float strength;
	public float attackSpeed;
	public float resistance;

	public int attackSpreadLevel = 1;
	public int attackDistanceLevel = 1;
	public int moveSpeedLevel = 1;
	public int strengthLevel = 1;
	public int attackSpeedLevel = 1;
	public int resistanceLevel = 1;
	public int maxHealthLevel = 1;

	private StatsManager statsManager;
	private Health health;

	void Start()
	{
		health = GetComponent<Health> ();
		statsManager = GameObject.FindGameObjectWithTag ("StatsManager").GetComponent<StatsManager> ();
	}

	void Update()
	{
		//set stats
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
	}

}
