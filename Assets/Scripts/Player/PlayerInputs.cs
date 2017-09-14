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
	public PlayerAction Block;

	public PlayerAction Purchase;

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
		Block = CreatePlayerAction ("Block");
		Move = CreateTwoAxisPlayerAction (Left, Right, Down, Up);

		Purchase = CreatePlayerAction("Purchase");
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
			Left.AddDefaultBinding (Key.A);
			Right.AddDefaultBinding (Key.D);
			Up.AddDefaultBinding (Key.W);
			Down.AddDefaultBinding (Key.S);

			BasicAttack.AddDefaultBinding(Key.J);
			BasicAttack.AddDefaultBinding(Mouse.LeftButton);

            Block.AddDefaultBinding(Key.K);
			Block.AddDefaultBinding(Mouse.RightButton);

            DashSlash.AddDefaultBinding (Key.L);
			DashSlash.AddDefaultBinding(Key.LeftShift);

			Purchase.AddDefaultBinding(Key.E);

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

			Block.AddDefaultBinding (InputControlType.LeftTrigger);
			BasicAttack.AddDefaultBinding (InputControlType.Action3);
			DashSlash.AddDefaultBinding (InputControlType.RightTrigger);

			Purchase.AddDefaultBinding(InputControlType.Action2);

			IncludeDevices.Add (device);
		}
	}
}
