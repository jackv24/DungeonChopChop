using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour 
{

	float moveSpeed;
	public float gravity = -9.8f;

	private PlayerInputs input;
	private CharacterController characterController;
	private PlayerInformation playerInformation;

	private Vector2 inputVector;
	private Vector3 moveVector;

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
	}
	
	// Update is called once per frame
	void Update () 
	{
		//sets movespeed to playerinformation movespeed;
		moveSpeed = playerInformation.moveSpeed;

		//checks if the player is on keyboard
		inputVector = input.Move;

		if (inputVector.magnitude > 1)
			inputVector.Normalize ();

		moveVector.x = inputVector.x * moveSpeed;
		moveVector.z = inputVector.y * moveSpeed;

		//rotate player
		if(inputVector.magnitude > 0.01f)
			transform.rotation = Quaternion.LookRotation(new Vector3(inputVector.x, 0, inputVector.y));

		//checks to see if the user is grounded, if not apply gravity
		if (!characterController.isGrounded) 
		{
			moveVector.y += gravity * Time.deltaTime;
		}
		//moves the player using the input axis and move speed
		characterController.Move (moveVector * Time.deltaTime);
	}
}
