using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Charms
{
	None,
	ThiefsCharm,
	DashCharm,
	StrengthCharm,
	ReacherCharm,
	ArmorBugCharm,
	DeathTouchCharm
};

public class PlayerCharm : MonoBehaviour {

	private PlayerInformation playerInfo;

	[Header("Thiefs Charm Values")]
	[Tooltip("2 = double")]
	public Sprite thiefCharmImg;
	public float pickUpIncrease = 1.25f;
	private float setPickUpIncrease;

	[Header("Dash Charm Values")]
	[Tooltip("2 = double")]
	public Sprite dashCharmImg;
	public float dashDistanceIncrease = 1.5f;
	private float setDashDistance;

	[Header("Strength Charm Values")]
	[Tooltip("2 = double")]
	public Sprite strengthCharmImg;
	public float damageIncrease = 1.1f;
	private float setDamageIncrease;

	[Header("Reacher Charm Values")]
	[Tooltip("2 = double")]
	public Sprite reacherCharmImg;

	[Header("Armor Charm Values")]
	[Tooltip("2 = double")]
	public Sprite armorCharmImg;

	[Header("Death touch Charm Values")]
	[Tooltip("2 = double")]
	public Sprite deathTouchCharmImg;

	//armor to do

	//death touch to do

	// Use this for initialization
	void Start () {
		playerInfo = GetComponent<PlayerInformation> ();


	}
	
	// Update is called once per frame
	void Update () {

	}
}
