using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
	public static Vector3 dungeonEntrancePos;
	public static int dungeonEntranceTile;

	public LevelGeneratorProfile profile;

	public int seed = 0;

	[Tooltip("Does this lead back to the overworld?")]
	public bool overworldEntrance = false;
    public bool resetTileAndPos = false;

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
			if (!overworldEntrance)
			{
				dungeonEntrancePos = transform.position - Vector3.forward * 2 + Vector3.up;
				dungeonEntranceTile = LevelGenerator.Instance.generatedTiles.IndexOf(LevelGenerator.Instance.currentTile);

				LevelVars.Instance.lastOverworldBiome = LevelGenerator.Instance.currentTile.Biome;

				LevelGenerator.Instance.RegenerateWithProfile(profile, seed);
			}
            else
            {
				if(resetTileAndPos)
                	LevelGenerator.Instance.RegenerateWithProfile(profile, seed);
				else
					LevelGenerator.Instance.RegenerateWithProfile(profile, seed, dungeonEntrancePos, dungeonEntranceTile);
            }
        }
	}
}
