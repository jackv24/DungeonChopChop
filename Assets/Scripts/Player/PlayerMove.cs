using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	public float moveSpeed;
	public float gravity = -9.8f;

	private PlayerInputs input;
	private CharacterController characterController;
	private PlayerInformation playerInformation;
	private Vector3 moveVector;

	// Use this for initialization
	void Start () 
	{
		playerInformation = GetComponent<PlayerInformation> ();
		input = InputManager.GetPlayerInput (playerInformation.playerIndex);
		characterController = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		moveVector.x = input.Move.X * moveSpeed;
		moveVector.z = input.Move.Y * moveSpeed;
		//checks to see if the user is grounded, if not apply gravity
		if (!characterController.isGrounded) 
		{
			moveVector.y += gravity * Time.deltaTime;
		}
		//moves the player using the input axis and move speed
		characterController.Move (moveVector * Time.deltaTime);
	}
}
