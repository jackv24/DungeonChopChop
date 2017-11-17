using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ControllerRumble : MonoBehaviour
{
	private static ControllerRumble Instance;

	public float shakeLengthMultiplier = 0.5f;

	private Dictionary<PlayerInputs, Coroutine> routines = new Dictionary<PlayerInputs, Coroutine>();

	void Awake()
	{
		Instance = this;
	}

	public static void RumbleController(PlayerInputs input, float magnitude, float length)
	{
		if(Instance)
		{
			if (Instance.routines.ContainsKey(input) && Instance.routines[input] != null)
			{
				Instance.StopCoroutine(Instance.routines[input]);
				input.device.StopVibration();
			}

			Instance.routines[input] = Instance.StartCoroutine(Instance.Rumble(input, magnitude, length));
		}
	}

	IEnumerator Rumble(PlayerInputs input, float magnitude, float length)
	{
		input.device.Vibrate(magnitude);

		yield return new WaitForSecondsRealtime(length * shakeLengthMultiplier);

		input.device.StopVibration();
	}
}
