﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerInputs : PlayerActionSet 
{
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;

	public PlayerTwoAxisAction Move;

	private InputDevice device;

	public PlayerInputs()
	{
		//set up player move axis
		Left = CreatePlayerAction ("Move Left");
		Right = CreatePlayerAction ("Move Right");
		Up = CreatePlayerAction ("Move Up");
		Down = CreatePlayerAction ("Move Down");
		Move = CreateTwoAxisPlayerAction (Left, Right, Down, Up);
	}

	public void AssignDevice(InputDevice inputDevice)
	{
		device = inputDevice;
	}

	public void SetupBindings()
	{
		if (device == null)
		{
			Left.AddDefaultBinding (Key.LeftArrow);
			Left.AddDefaultBinding (Key.A);

			Right.AddDefaultBinding (Key.RightArrow);
			Right.AddDefaultBinding (Key.D);

			Up.AddDefaultBinding (Key.UpArrow);
			Up.AddDefaultBinding (Key.W);

			Down.AddDefaultBinding (Key.DownArrow);
			Down.AddDefaultBinding (Key.S);
		}
		else
		{
			Left.AddDefaultBinding (InputControlType.DPadLeft);
			Left.AddDefaultBinding (InputControlType.LeftStickLeft);

			Right.AddDefaultBinding (InputControlType.DPadRight);
			Right.AddDefaultBinding (InputControlType.LeftStickRight);

			Up.AddDefaultBinding (InputControlType.DPadUp);
			Up.AddDefaultBinding (InputControlType.LeftStickUp);

			Down.AddDefaultBinding (InputControlType.DPadDown);
			Down.AddDefaultBinding (InputControlType.LeftStickDown);
		}
	}
}
