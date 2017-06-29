using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	public int playerIndex = 0;

	public float moveSpeed;
	public float gravity = -9.8f;

	private PlayerInputs input;
	private CharacterController characterController;
	private Vector3 moveVector;

	// Use this for initialization
	void Start () 
	{
		input = InputManager.GetPlayerInput (playerIndex);
		characterController = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		moveVector.x = input.Move.X * moveSpeed;
		moveVector.z = input.Move.Y * moveSpeed;

		if (!characterController.isGrounded) 
		{
			moveVector.y += gravity * Time.deltaTime;
		}

		characterController.Move (moveVector * Time.deltaTime);
	}
}
