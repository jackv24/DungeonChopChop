﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	int playersReady = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//sets up single player with a controller
	void SetupControllerSingle()
	{
		InputManager.Instance.playerInput.Add (new PlayerInputs ());
		InputManager.Instance.playerInput [playersReady].SetupBindings ();
		InputManager.Instance.playerInput [playersReady].AssignDevice (InControl.InputManager.Devices[0]);
		InputManager.Instance.playerInput [playersReady].SetupBindings ();
	}

	//sets up co op with only one controller
	void SetUpCoOpOneController()
	{
		InputManager.Instance.playerInput.Add (new PlayerInputs ());
		InputManager.Instance.playerInput [playersReady].SetupBindings ();
		playersReady++;
		InputManager.Instance.playerInput.Add (new PlayerInputs ());
		InputManager.Instance.playerInput [playersReady].AssignDevice (InControl.InputManager.Devices [0]);
		InputManager.Instance.playerInput [playersReady].SetupBindings ();
	}

	//sets up co op with two controllers
	void SetUpCoOpTwoControllers()
	{
		InputManager.Instance.playerInput.Add (new PlayerInputs ());
		InputManager.Instance.playerInput [playersReady].SetupBindings ();
		InputManager.Instance.playerInput [playersReady].AssignDevice (InControl.InputManager.Devices [0]);
		InputManager.Instance.playerInput [playersReady].SetupBindings ();
		playersReady++;
		InputManager.Instance.playerInput.Add (new PlayerInputs ());
		InputManager.Instance.playerInput [playersReady].AssignDevice (InControl.InputManager.Devices [1]);
		InputManager.Instance.playerInput [playersReady].SetupBindings ();
	}

	//sets up everything if single player
	public void SinglePlayer()
	{
		if (InControl.InputManager.Devices.Count > 0) 
		{
			SetupControllerSingle ();
		} 
		else 
		{
			InputManager.Instance.playerInput.Add (new PlayerInputs ());
			InputManager.Instance.playerInput [playersReady].SetupBindings ();
		}
	}

	//sets up everything if co op
	public void CoOp()
	{
        int temp = 0;
        for (int i = 0; i < InControl.InputManager.Devices.Count; i++)
        {
            if (InControl.InputManager.Devices[i].Name != "Unknown Device")
            {
                temp++;
            }
        }

		if (temp > 0 && temp < 2) 
		{
			SetUpCoOpOneController ();
		} 
        else if (temp > 1) 
		{
			SetUpCoOpTwoControllers ();
		}
	}
}
