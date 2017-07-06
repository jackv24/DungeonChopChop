using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerInputs : PlayerActionSet 
{
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;
	public PlayerAction BasicAttack;
	public PlayerAction DashSlash;

	public PlayerTwoAxisAction Move;

	public InputDevice device = null;

	public bool isKeyboard = false;

	public PlayerInputs()
	{
		//set up player move axis
		Left = CreatePlayerAction ("Move Left");
		Right = CreatePlayerAction ("Move Right");
		Up = CreatePlayerAction ("Move Up");
		Down = CreatePlayerAction ("Move Down");
		BasicAttack = CreatePlayerAction ("Basic Attack");
		DashSlash = CreatePlayerAction ("Dash Slash");
		Move = CreateTwoAxisPlayerAction (Left, Right, Down, Up);
	}

	public void AssignDevice(InputDevice inputDevice)
	{
		device = inputDevice;
	}

	public void SetupBindings()
	{
		//sets the bindings if the device is a keyboard
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

			BasicAttack.AddDefaultBinding (Mouse.LeftButton);
			DashSlash.AddDefaultBinding (Mouse.RightButton);

			isKeyboard = true;
		}
		else
		{
			//sets the bindings if the device is a controller
			Left.AddDefaultBinding (InputControlType.DPadLeft);
			Left.AddDefaultBinding (InputControlType.LeftStickLeft);

			Right.AddDefaultBinding (InputControlType.DPadRight);
			Right.AddDefaultBinding (InputControlType.LeftStickRight);

			Up.AddDefaultBinding (InputControlType.DPadUp);
			Up.AddDefaultBinding (InputControlType.LeftStickUp);

			Down.AddDefaultBinding (InputControlType.DPadDown);
			Down.AddDefaultBinding (InputControlType.LeftStickDown);

			BasicAttack.AddDefaultBinding (InputControlType.Action3);
			DashSlash.AddDefaultBinding (InputControlType.RightTrigger);

			IncludeDevices.Add (device);
		}
	}
}
