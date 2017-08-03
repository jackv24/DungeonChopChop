using System.Collections;
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

	[Header("Stat Levels")]
	public int attackSpreadLevel = 1;
	public int attackDistanceLevel = 1;
	public int moveSpeedLevel = 1;
	public int strengthLevel = 1;
	public int attackSpeedLevel = 1;
	public int resistanceLevel = 1;
	public int maxHealthLevel = 1;

	[Header("Charm Values")]
	public bool canDash = false;
	public bool canMagnetize = false;

	[Header("Charm")]
	public int charmAmount;
	public List<Charm> currentCharms =  new List<Charm>();

	private StatsManager statsManager;
	private Health health;

	private int prevMoveSpeedLevel = 0;

	private WeaponStats currentWeaponStats;

	void Start()
	{
		health = GetComponent<Health> ();

		GameObject s = GameObject.FindGameObjectWithTag("StatsManager");
		if(s)
			statsManager = s.GetComponent<StatsManager> ();

		PickupCharm (null);
	}

	void Update()
	{
		//set stat values depending on level
		if (statsManager) 
		{
			attackMinAngle = statsManager.GetStatValue (StatName.AttackSpeed, attackSpreadLevel); 
			attackDistance = statsManager.GetStatValue (StatName.Range, attackDistanceLevel);
			strength = statsManager.GetStatValue (StatName.Strength, strengthLevel);
			attackSpeed = statsManager.GetStatValue (StatName.AttackSpeed, attackSpeedLevel);
			resistance = statsManager.GetStatValue (StatName.Resistance, resistanceLevel);
			health.maxHealth = (int)statsManager.GetStatValue (StatName.maxHealth, maxHealthLevel); 
			if (moveSpeedLevel != prevMoveSpeedLevel) {
				moveSpeed = statsManager.GetStatValue (StatName.RunSpeed, moveSpeedLevel);
			}
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
		prevMoveSpeedLevel = moveSpeedLevel;


		//if charm == Magnetic charm
		if (canMagnetize) 
		{
			MagnetizeItems ();
		}
	}

	public void PickupCharm(Charm charm)
	{
		if (charm) {
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
		foreach (CharmImage img in charmImg) {
			if (playerIndex == img.id)
				img.UpdateCharms (this);
		}
	}

	//-------------------------- Charm functions

	void MagnetizeItems()
	{
		//gets the magnetic charm
		foreach (Charm charm in currentCharms) 
		{
			MagneticCharm m = (MagneticCharm)charm;
			if (m) 
			{
				//gets all items in radius
				Collider[] items = Physics.OverlapSphere (transform.position, m.magnetizeRadius, m.layerMask);
				if (items.Length > 0) 
				{
					foreach (Collider item in items) 
					{
						//moves those items towards the player
						item.transform.position = Vector3.MoveTowards (item.transform.position, transform.position, m.absorbSpeed * Time.deltaTime);
					}
				}
			}
		}

	}
}
