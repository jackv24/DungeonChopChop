using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputManager : MonoBehaviour 
{
	public static InputManager Instance;

	PlayerInputs[] playerInput;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		playerInput = new PlayerInputs[2];

		playerInput [0] = new PlayerInputs ();
		playerInput [1] = new PlayerInputs ();

		playerInput [0].SetupBindings ();

		playerInput [1].AssignDevice (InControl.InputManager.Devices[0]);
		playerInput [1].SetupBindings ();
	}

	public static PlayerInputs GetPlayerInput(int index)
	{
		return Instance.playerInput[index];
	}


}
