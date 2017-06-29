using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ReadyUpManager : MonoBehaviour
{

	int playersReady = 0;

	void Update()
	{
		checkPlayersInput ();
	}

	void checkPlayersInput()
	{
		bool sameDevice = false;
		//checks to see if the user(s) is using a keyboard
		if (Input.GetKeyDown(KeyCode.Space)) 
		{
			foreach (PlayerInputs input in InputManager.Instance.playerInput) 
			{
				if (input != null) 
				{
					if (input.isKeyboard) 
					{
						sameDevice = true;
					}
				}
			}
			if (!sameDevice) 
			{
				InputManager.Instance.playerInput.Add (new PlayerInputs ());
				InputManager.Instance.playerInput [playersReady] = new PlayerInputs ();
				InputManager.Instance.playerInput [playersReady].SetupBindings ();
				playersReady++;
			}
		}
		//checks to see if the user(s) is using a controller
		if (InControl.InputManager.ActiveDevice.AnyButton.WasPressed) 
		{
			foreach (PlayerInputs input in InputManager.Instance.playerInput) 
			{
				if (input != null) 
				{
					if (input.device == InControl.InputManager.ActiveDevice) 
					{
						sameDevice = true;
					}
				}
			}
			if (!sameDevice) 
			{
				InputManager.Instance.playerInput.Add (new PlayerInputs ());
				InputManager.Instance.playerInput [playersReady] = new PlayerInputs ();
				InputManager.Instance.playerInput [playersReady].AssignDevice (InControl.InputManager.ActiveDevice);
				InputManager.Instance.playerInput [playersReady].SetupBindings ();
				playersReady++;
			}
		}
	}
}
