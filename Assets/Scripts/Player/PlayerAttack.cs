using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour 
{
	[Header("Attack vars")]
	public float attackMinAngle = 130;
	public float attackDistance = 5;

	[Header("Combo Vars")]
	public float timeInbetween;
	[Tooltip("The amount added to the combo counter (60 times a second)")]
	public float comboIncrease = .04f;

	[Header("Rapid Slash Vars")]
	public int slashAmount;
	public float rapidSlashCooldown;
	[Tooltip("The amount added to the rapid slash cooldown (60 times a second)")]
	public float rapidSlashIncrease = .04f;

	bool canAttack = true;

	private PlayerInputs input;
	private PlayerInformation playerInformation;

	private int comboAmount;

	private float comboCounter;
	private float rapidSlashCounter;

	private bool rapidSlashCoolingDown = false;
	private bool comboStarted = false;

	void Start () 
	{
		playerInformation = GetComponent<PlayerInformation> ();
		input = InputManager.GetPlayerInput (playerInformation.playerIndex);
	}


	void Update()
	{
		//check if can actually attack
		if (canAttack) 
		{
			//do basic attack
			if (input.BasicAttack.WasPressed) 
			{
				CheckCombo ();
				//if combo is equal to or greater than 3, do rapid slash
				if (comboAmount >= 3) 
				{
					//rapid slash
					doRapidSlash ();
				} 
				else 
				{
					//basic slash
					doSlash ();
				}
			} 
			else if (input.DashSlash.WasPressed) 
			{
				//dash slash
				doDashSlash ();
			} 
			else if (input.BasicAttack) 
			{
				//block
				doBlock ();
			}
		}
	}

	void FixedUpdate()
	{
		if (comboStarted) 
		{
			CountCombo ();
			//check if combo has ended
			if (comboCounter > timeInbetween)
			{
				ResetCombo ();
			}
		}

		if (rapidSlashCoolingDown) 
		{
			RapidSlashCooldownCounter ();
			//check if rapid slash cooldown is over
			if (rapidSlashCounter > rapidSlashCooldown) 
			{
				ResetRapidSlash ();
			}
		}
	}

	//-------------------------- Combo stuff

	void CheckCombo()
	{
		comboStarted = true;
		//if you attack quick enough, it is counted as a combo
		if (comboCounter < timeInbetween) 
		{
			AddCombo ();
		} 
		//if you attack to late, it restarts the combo
		else 
		{
			ResetCombo ();
		}
	}

	void ResetCombo()
	{
		//resets everything to do with combo
		comboAmount = 0;
		comboCounter = 0;
		comboStarted = false;
	}

	void AddCombo()
	{
		//adds 1 to combo amount
		comboAmount++;
		comboCounter = 0;
	}
		

	//-------------------------- Block


	void doBlock()
	{
		//do block things


		Debug.Log ("Blocking");
	}


	//-------------------------- Attacks

	void doSlash()
	{
		//do slash things
		Collider[] colliders = Physics.OverlapSphere(transform.position, attackDistance);
		foreach (Collider col in colliders) {
			if (col != null && col) {
				if (col.gameObject.layer == 11) {
					if (col.GetComponent<Health> ()) {
						float angle = Vector3.Angle (transform.forward, transform.position - col.transform.position);
						if (angle > attackMinAngle) {
							col.GetComponent<Health> ().AffectHealth (-20);
							Debug.Log ("hit enemy");
						}
					}
				}
			}
		}

		Debug.Log ("Basic Slash");
	}

	void doRapidSlash()
	{
		//do rapid slash things

		rapidSlashCoolingDown = true;
		canAttack = false;
		ResetCombo ();
		Debug.Log ("Rapid Slash");
	}

	void ResetRapidSlash()
	{
		rapidSlashCoolingDown = false;
		rapidSlashCounter = 0;
		canAttack = true;
	}

	void doDashSlash()
	{
		//do flash
		Debug.Log("Dash Slash");
	}


	//-------------------------- Counters

	void CountCombo()
	{
		comboCounter += comboIncrease;
	}

	void RapidSlashCooldownCounter()
	{
		rapidSlashCounter += rapidSlashIncrease;
	}

}
