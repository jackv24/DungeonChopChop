using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
	public LevelGeneratorProfile profile;

	public int seed = 0;

	[Tooltip("Does this lead back to the overworld?")]
	public bool overworldEntrance = false;

	void OnEnable()
	{
		//Set profile and seed as overworld if desired
		if(overworldEntrance)
		{
			profile = LevelVars.Instance.levelData.overworldProfile;
			seed = LevelVars.Instance.levelData.overworldSeed;
		}

		//We are in a dungeon if this entrance leads back to the overworld
		LevelVars.Instance.levelData.inDungeon = overworldEntrance;
	}

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
