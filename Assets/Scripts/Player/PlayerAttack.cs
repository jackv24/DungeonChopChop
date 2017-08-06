using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour 
{
	float attackMinAngle = 130;
	float attackDistance = 5;

	[Header("Combo Vars")]
	public float timeInbetween;
	[Tooltip("The amount added to the combo counter (60 times a second)")]
	public float comboIncrease = .04f;

	[Header("Rapid Slash Vars")]
	public int slashAmount;
	public float rapidSlashCooldown;
	[Tooltip("The amount added to the rapid slash cooldown (60 times a second)")]
	public float rapidSlashIncrease = .04f;

	[Header("Dash Vars")]
	public float dashTime = 1.0f;
	public float dashSpeed = 5.0f;
	public float dashCooldown = 0.5f;

	bool canAttack = true;

	private PlayerInputs input;
	private PlayerMove playerMove;
	private PlayerInformation playerInformation;
	private CharacterController characterController;

	public ShieldStats shield;

	private int comboAmount;

	private float comboCounter;
	private float rapidSlashCounter;
	private float normalMoveSpeed;

	private bool rapidSlashCoolingDown = false;
	private bool comboStarted = false;

	void Start () 
	{
		characterController = GetComponent<CharacterController> ();
		playerInformation = GetComponent<PlayerInformation> ();
		playerMove = GetComponent <PlayerMove> ();

		if (InputManager.Instance) 
		{
			input = InputManager.GetPlayerInput (playerInformation.playerIndex);
		} 
		else 
		{
			input = new PlayerInputs ();
			input.SetupBindings ();
		}
	}


	void Update()
	{
		if (Input.GetKeyDown (KeyCode.P)) {
			foreach (Charm charm in playerInformation.currentCharms) {
				charm.Pickup (playerInformation);
			}
		}
		//sets attack values
		attackMinAngle = playerInformation.attackMinAngle;
		attackDistance = playerInformation.attackDistance;
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
				doDash ();
			} 
			else if (input.Block.WasPressed) 
			{
				//block
				if (shield) {
					doBlock ();
				}
			}
			else if (input.Block.WasReleased) 
			{
				//block
				if (shield) {
					stopBlock ();
				}
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
		normalMoveSpeed = playerInformation.moveSpeed;
		playerInformation.moveSpeed = playerInformation.moveSpeed * shield.speedDamping;

		//Debug.Log ("Blocking");
	}

	void stopBlock()
	{
		//stop block things
		playerInformation.moveSpeed = normalMoveSpeed;
	}


	//-------------------------- Attacks

	float criticalHit()
	{
		if (playerInformation.HasCharmFloat ("critChance")) 
		{
			float randomPercent = Random.Range (0, 101);
			if (randomPercent >= playerInformation.GetCharmFloat("critChance")) 
			{
				return playerInformation.GetCharmFloat("critMultiplier");
			} 
			else 
			{
				return 1.0f;
			}
		}
		return 1.0f;
	}

	public void knockback(GameObject obj)
	{
		obj.GetComponent<Rigidbody> ().AddForce (transform.forward * playerInformation.knockback * playerInformation.GetCharmFloat("kockbackMultiplier"), ForceMode.Impulse);
	}

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
							col.GetComponent<Health> ().AffectHealth (-20 * criticalHit());
							//Debug.Log ("hit enemy");
						}
					}
				}
			}
		}

		//Debug.Log ("Basic Slash");
	}

	void doRapidSlash()
	{
		//do rapid slash things

		rapidSlashCoolingDown = true;
		canAttack = false;
		ResetCombo ();
		//Debug.Log ("Rapid Slash");
	}

	void ResetRapidSlash()
	{
		rapidSlashCoolingDown = false;
		rapidSlashCounter = 0;
		canAttack = true;
	}

	void doDash()
	{
		StartCoroutine(dash());
		StartCoroutine (dashCooldownTimer ());
	}

	IEnumerator dashCooldownTimer()
	{
		int i = 0;
		float cooldown = dashCooldown * playerInformation.GetCharmFloat ("dashCooldown");

		while (i < cooldown)
		{
			yield return new WaitForSeconds (1);
			i++;
		}
	}

	IEnumerator dash()
	{
		playerMove.enabled = false;
		Vector3 startingPos = transform.position;
		float elapsedTime = 0;
		float timer = dashTime * playerInformation.GetCharmFloat ("dashTime");
		while (elapsedTime < timer) {
			characterController.Move(transform.forward * dashSpeed * playerInformation.GetCharmFloat("dashSpeed") * Time.deltaTime);
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}
		playerMove.enabled = true;
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
