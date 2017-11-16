using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputManager : MonoBehaviour 
{
	public static InputManager Instance;

	public List<PlayerInputs> playerInput;

    private bool paused = true;

    void Awake()
	{
        DontDestroyOnLoad(this);

		playerInput = new List<PlayerInputs>();
		Instance = this;
	}

	void Update()
	{
        //If there is a levelgenerator present, we must be in-game
        if (LevelGenerator.Instance)
        {
            if (Pause.Instance.paused != paused)
            {
                paused = Pause.Instance.paused;

                if (paused)
                {
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.visible = false;
                }
            }
        }
        else
        {
            Cursor.visible = true;
        }
    }

	public static PlayerInputs GetPlayerInput(int index)
	{
        	return Instance.playerInput[index];
	}
}
