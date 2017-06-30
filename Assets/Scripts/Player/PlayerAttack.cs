using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour 
{
	[Header("Combo Vars")]
	public float timeInbetween;
	[Tooltip("The amount added to the combo counter (60 times a second)")]
	public float comboIncrease = .04f;

	[Header("Rapid Slash Vars")]
	public int slashAmount;


	private PlayerInputs input;
	private PlayerInformation playerInformation;

	int comboAmount;
	float comboCounter;
	bool comboStarted = false;

	void Start () 
	{
		playerInformation = GetComponent<PlayerInformation> ();
		input = InputManager.GetPlayerInput (playerInformation.playerIndex);
	}


	void Update()
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
				Slash();
			}
		}
	}

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

	void FixedUpdate()
	{
		if (comboStarted) 
		{
			CountCombo ();
		}
		//check if combo has ended
		if (comboCounter > timeInbetween)
		{
			ResetCombo ();
		}
	}

	void Slash()
	{
		//do slash things


		Debug.Log ("Basic Slash");
	}

	void doRapidSlash()
	{
		//do rapid slash things


		ResetCombo ();
		Debug.Log ("Rapid Slash");
	}

	void CountCombo()
	{
		comboCounter += comboIncrease;
	}

}
