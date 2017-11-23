using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputManager : MonoBehaviour 
{
	public static InputManager Instance;

	public List<PlayerInputs> playerInput;

    private bool hidden = false;

    void Awake()
	{
        DontDestroyOnLoad(this);

		playerInput = new List<PlayerInputs>();
		Instance = this;
	}

	void Update()
	{
        if(!hidden && (Input.anyKeyDown || (InControl.InputManager.ActiveDevice != null && InControl.InputManager.ActiveDevice.AnyButtonWasPressed)))
		{
			hidden = true;

			Cursor.visible = false;
		}
		else if(Mathf.Abs(Input.GetAxisRaw("Mouse X")) > 0.1f)
		{
			hidden = false;

			Cursor.visible = true;
		}
    }
}
