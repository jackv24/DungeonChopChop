using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputManager : MonoBehaviour 
{
	public static InputManager Instance;

	public List<PlayerInputs> playerInput;

	void Awake()
	{
        DontDestroyOnLoad(this);

		playerInput = new List<PlayerInputs>();
		Instance = this;
	}

	public static PlayerInputs GetPlayerInput(int index)
	{
		return Instance.playerInput[index];
	}

    void Update()
    {
        
    }
}
