using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
	public LevelGeneratorProfile profile;

	public int seed = 0;

	private void OnTriggerEnter(Collider col)
	{
		//If player walked into this door
		if (col.gameObject.layer == 14 && col.GetType() == typeof(CharacterController))
		{
			StartCoroutine(EnterDelayed());
		}
	}

	IEnumerator EnterDelayed()
	{
		yield return new WaitForEndOfFrame();

		if (LevelGenerator.Instance && profile)
		{
			LevelGenerator.Instance.RegenerateWithProfile(profile, seed);
		}
	}
}
