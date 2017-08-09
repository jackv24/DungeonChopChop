using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour 
{

	float moveSpeed;
	public float gravity = -9.8f;
	public float acceleration = 1;
	public float inMudSpeed = 2.5f;

	private bool allowMove = true;

	private PlayerInputs input;
	private CharacterController characterController;
	private PlayerInformation playerInformation;

	private float speed;

	private Vector2 inputVector;
	private Vector3 targetMoveVector;
	private Vector3 fromMoveVector = Vector3.zero;
	private bool onIce = false;
	private float originalMoveSpeed = 0;

	// Use this for initialization
	void Start () 
	{
		playerInformation = GetComponent<PlayerInformation> ();
		if (InputManager.Instance) 
		{
			input = InputManager.GetPlayerInput (playerInformation.playerIndex);
		} 
		else 
		{
			input = new PlayerInputs ();
			input.SetupBindings ();
		}
		characterController = GetComponent<CharacterController> ();

		//Wait until level generation is done to allow movement
		if(LevelGenerator.Instance)
		{
			allowMove = false;

			LevelGenerator.Instance.OnGenerationFinished += delegate { allowMove = true; };
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!allowMove)
			return;

		//sets movespeed to playerinformation movespeed;
		moveSpeed = playerInformation.moveSpeed;

		//sets the input vector
		inputVector = input.Move;

		if (inputVector.magnitude > 1)
			inputVector.Normalize ();

		targetMoveVector.x = inputVector.x * moveSpeed * playerInformation.GetCharmFloat("moveSpeedMultiplier");
		targetMoveVector.z = inputVector.y * moveSpeed * playerInformation.GetCharmFloat("moveSpeedMultiplier");

		if (CameraFollow.Instance)
			targetMoveVector = CameraFollow.Instance.ValidateMovePos(transform.position, targetMoveVector);

		//rotate player
		if(inputVector.magnitude > 0.01f)
			transform.rotation = Quaternion.LookRotation(new Vector3(inputVector.x, 0, inputVector.y));

		//checks to see if the user is grounded, if not apply gravity
		if (!characterController.isGrounded) 
		{
			targetMoveVector.y += gravity * Time.deltaTime;
		}
			
		//moves the player using the input axis and move speed
		//checks if the player is on ice or not
		if (!onIce)
		{
			characterController.Move (targetMoveVector * Time.deltaTime);
		}
		else 
		{
			fromMoveVector = Vector3.Lerp (fromMoveVector, targetMoveVector, acceleration * Time.deltaTime);
			characterController.Move (fromMoveVector * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Mud") 
		{
			originalMoveSpeed = playerInformation.moveSpeed;
		}
	}		

	void OnTriggerStay(Collider col)
	{
		//checks if the floor is ice
		if (col.tag == "Ice") 
		{
			onIce = true;
		} 
		else if (col.tag == "Mud")
		{
			playerInformation.moveSpeed = inMudSpeed;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Ice")
		{
			fromMoveVector = Vector3.zero;
			onIce = false;
		} 
		else if (col.tag == "Mud")
		{
			playerInformation.moveSpeed = originalMoveSpeed;
		}
	}

}
