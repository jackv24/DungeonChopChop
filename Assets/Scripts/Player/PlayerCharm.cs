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

	public Charms Charm
	{
		get
		{
			return playerInfo.currentCharm;
		}

		set
		{
			playerInfo.currentCharm = value;


		}
	}

	[Header("Thiefs Charm Values")]
	[Tooltip("2 = double")]
	public float pickUpIncrease = 1.25f;
	private float setPickUpIncrease;

	[Header("Dash Charm Values")]
	[Tooltip("2 = double")]
	public float dashDistanceIncrease = 1.5f;
	private float setDashDistance;

	[Header("Strength Charm Values")]
	[Tooltip("2 = double")]
	public float damageIncrease = 1.1f;
	private float setDamageIncrease;

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
