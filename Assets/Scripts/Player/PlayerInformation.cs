﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour 
{
	[Header("Player Values")]
	public int playerIndex = 0;
	public float attackMinAngle = 130;
	public float attackDistance = 5;
	public float moveSpeed;
	public float strength;
	public float attackSpeed;
	public float resistance;
	public float knockback;

	[Header("Stat Levels")]
	public int attackSpreadLevel = 1;
	public int attackDistanceLevel = 1;
	public int moveSpeedLevel = 1;
	public int strengthLevel = 1;
	public int attackSpeedLevel = 1;
	public int resistanceLevel = 1;
	public int maxHealthLevel = 1;
	public int knockbackLevel = 1;

	[Header("Charm")]
	public int charmAmount;
	public List<Charm> currentCharms =  new List<Charm>();

	private StatsManager statsManager;
	private Health health;

	private int prevMoveSpeedLevel = 0;
	private int prevAttackSpreadLevel = 0;
	private int prevAttackDistanceLevel = 0;
	private int prevStrengthLevel = 0;
	private int prevAttackSpeedLevel = 0;
	private int prevResistanceLevel = 0;
	private int prevMaxHealthLevel = 0;
	private int prevKnockbackLevel = 0;

	private WeaponStats currentWeaponStats;

	private Dictionary<string, float> charmFloats = new Dictionary<string, float>();
	private Dictionary<string, bool> charmBools = new Dictionary<string, bool>();
	private LayerMask layerMask;

	void Start()
	{
		health = GetComponent<Health> ();

		GameObject s = GameObject.FindGameObjectWithTag("StatsManager");
		if(s)
			statsManager = s.GetComponent<StatsManager> ();

		if (LevelGenerator.Instance)
		{
			LevelGenerator.Instance.OnTileEnter += RegenHealth;
			LevelGenerator.Instance.OnTileEnter += SpeedBuff;
		}

		PickupCharm (null);
	}

	void Update()
	{
		//set stat values depending on level
		if (statsManager) 
		{
			if (attackSpreadLevel != prevAttackSpreadLevel)
				attackMinAngle = statsManager.GetStatValue (StatName.AttackSpeed, attackSpreadLevel); 
			if (attackDistanceLevel != prevAttackDistanceLevel)
				attackDistance = statsManager.GetStatValue (StatName.Range, attackDistanceLevel);
			if (strengthLevel != prevStrengthLevel)
				strength = statsManager.GetStatValue (StatName.Strength, strengthLevel);
			if (attackSpeedLevel != prevAttackSpeedLevel)
				attackSpeed = statsManager.GetStatValue (StatName.AttackSpeed, attackSpeedLevel);
			if (resistanceLevel != prevResistanceLevel)
				resistance = statsManager.GetStatValue (StatName.Resistance, resistanceLevel);
			if (maxHealthLevel != prevMaxHealthLevel)
				health.maxHealth = (int)statsManager.GetStatValue (StatName.maxHealth, maxHealthLevel); 
			if (moveSpeedLevel != prevMoveSpeedLevel) 
				moveSpeed = statsManager.GetStatValue (StatName.RunSpeed, moveSpeedLevel);
			if (knockbackLevel != prevKnockbackLevel) 
				knockback = statsManager.GetStatValue (StatName.Knockback, knockbackLevel);
			
		}
		//sets stats depending on weapon values
		if (currentWeaponStats) 
		{
			attackSpreadLevel = currentWeaponStats.spreadLevel;
			attackSpeedLevel = currentWeaponStats.speedLevel;
			attackDistanceLevel = currentWeaponStats.distanceLevel;
			knockbackLevel = currentWeaponStats.knockBackLevel;
		} 
		else 
		{
			currentWeaponStats = GetComponentInChildren<WeaponStats> ();
		}

		//assigns previous level to current stat level
		prevMoveSpeedLevel = moveSpeedLevel;
		prevAttackSpreadLevel = attackSpreadLevel;
		prevAttackDistanceLevel = attackDistanceLevel;
		prevStrengthLevel = strengthLevel;
		prevResistanceLevel = resistanceLevel;
		prevMaxHealthLevel = maxHealthLevel;


		//if charm == Magnetic charm
		MagnetizeItems ();
	}

	public void PickupCharm(Charm charm)
	{
		if (charm) 
		{
			//adds a charm to the start of the list
			currentCharms.Insert (0, charm);
			//checks to see if the amount of charms the player has is greater then the amount they can hold
			if (currentCharms.Count > charmAmount) {
				//creates a new charm and adds it to the list
				Charm oldCharm = currentCharms [currentCharms.Count - 1];
				//removes the older charm
				currentCharms.Remove (oldCharm);

				oldCharm.Drop (this);
			}

			charm.Pickup (this);
		}
		CharmImage[] charmImg = FindObjectsOfType<CharmImage> ();
		//loops through each charm and updates the ui
		foreach (CharmImage img in charmImg) 
		{
			if (playerIndex == img.id)
				img.UpdateCharms (this);
		}
	}

	public void SetLayerMask(LayerMask lm)
	{
		layerMask = lm;
	}

	public void SetCharmFloat(string key, float value)
	{
		charmFloats[key] = value;
	}

	public void SetCharmBool(string key, bool value)
	{
		charmBools[key] = value;
	}

	public float GetCharmFloat(string key)
	{
		if (charmFloats.ContainsKey(key))
			return charmFloats[key];
		else
			return 1.0f;
	}

	public bool GetCharmBool(string key)
	{
		if (charmBools.ContainsKey (key))
			return charmBools [key];
		else
			return false;
	}

	public bool HasCharmFloat(string key)
	{
		return charmFloats.ContainsKey (key);
	}

	public bool HasCharmBool(string key)
	{
		return charmBools.ContainsKey (key);
	}

	//-------------------------- Charm functions

	void MagnetizeItems()
	{
		if (HasCharmBool ("absorb")) {
			if (GetCharmBool ("absorb")) 
			{
				//gets the magnetic charm
				foreach (Charm charm in currentCharms) 
				{
					//gets all items in radius
					Collider[] items = Physics.OverlapSphere (transform.position, GetCharmFloat ("radialValue"), layerMask);
					if (items.Length > 0) 
					{
						foreach (Collider item in items) 
						{
							//moves those items towards the player
							item.transform.position = Vector3.MoveTowards (item.transform.position, transform.position, GetCharmFloat ("radialAbsorbSpeed") * Time.deltaTime);
						}
					}
				}
			}
		}
	}

	private int roomAmount = 0;

	void RegenHealth()
	{
		if (HasCharmFloat ("regenRooms")) {
			roomAmount++;

			int maxRoomAmount = Mathf.CeilToInt (GetCharmFloat ("regenRooms"));

			if (roomAmount >= maxRoomAmount) {
				roomAmount = 0;
				health.health += GetCharmFloat ("regenAmount");
			}
		}
	}

	void SpeedBuff()
	{
		if (HasCharmFloat ("speedBuffTime")) {
			StartCoroutine (SpeedBuffForTime (GetCharmFloat ("speedBuffTime"), GetCharmFloat ("speedBuff")));
		}
	}

	IEnumerator SpeedBuffForTime(float time, float multiplier)
	{
		float speed = moveSpeed;
		moveSpeed *= multiplier;

		yield return new WaitForSeconds (time);

		moveSpeed = speed;
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.transform.tag == "Enemy") 
		{
			if (HasCharmFloat ("burnTickTime")) 
			{
				if (!col.gameObject.GetComponent<Health> ().isBurned) 
				{
					if (col.transform.GetComponent<Health> ()) 
					{
						col.transform.GetComponent<Health> ().SetBurned (GetCharmFloat ("burnTickDamage"), GetCharmFloat ("burnTickTotalTime"), GetCharmFloat ("burnTickTime"));
					}
				}
			}
			if (!col.gameObject.GetComponent<Health> ().isPoisoned) 
			{
				if (HasCharmFloat ("poisonTickTime")) 
				{
					if (col.transform.GetComponent<Health> ()) 
					{
						col.transform.GetComponent<Health> ().SetPoison (GetCharmFloat ("poisonTickDamage"), GetCharmFloat ("poisonTickTotalTime"), GetCharmFloat ("poisonTickTime"));
					}
				}
			}
		}
	}

}
