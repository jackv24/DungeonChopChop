using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon", menuName = "Data/Dungeon Generator Profile")]
public class DungeonGeneratorProfile : LevelGeneratorProfile
{
	[Header("Dungeon Items")]
	public BaseItem forestItem;
	public BaseItem iceItem;
	public BaseItem desertItem;
	public BaseItem fireItem;

	public override void Generate(LevelGenerator levelGenerator)
	{
		//Replace layout tiles with dungeon tiles
		foreach(LevelTile tile in levelGenerator.generatedTiles)
		{
			tile.Replace(LevelTile.Biomes.Dungeon);
		}

		//Get the biome that this dungeon was entered from
		LevelTile.Biomes biome = LevelVars.Instance.lastOverworldBiome;

		///Spawn dungeon key and chest
		//Keep list of potential places to spawn
		List<DungeonKeyTile> potentialTiles = new List<DungeonKeyTile>();

		//Get tiles that can be replaced from level generator
		foreach(LevelTile tile in levelGenerator.generatedTiles)
		{
			if (tile.currentGraphic)
			{
				DungeonKeyTile key = tile.currentGraphic.GetComponent<DungeonKeyTile>();

				if (key)
					potentialTiles.Add(key);
			}
		}

		if (potentialTiles.Count >= 2)
		{
			DungeonKeyTile keyTile = null;
			DungeonKeyTile chestTile = null;

			//Key tile can be any random potential tile
			keyTile = potentialTiles[Random.Range(0, potentialTiles.Count)];
			//Key tile should not be considered for chest tile
			potentialTiles.Remove(keyTile);

			//Chest tile should be furthest away from key tile
			float furthestDistance = 0;
			foreach (DungeonKeyTile tile in potentialTiles)
			{
				float distance = Vector3.Distance(tile.transform.position, keyTile.transform.position);

				if (distance > furthestDistance)
				{
					furthestDistance = distance;

					chestTile = tile;
				}
			}

			//Make sure there is a key/chest pair
			if (!keyTile)
			{
				Debug.LogWarning("Did not spawn a dungeon <b>key</b> tile!");

				succeeded = false;
			}
			else
				keyTile.Replace(DungeonKeyTile.Type.Key);

			if (!chestTile)
			{
				Debug.LogWarning("Did not spawn a dungeon <b>chest</b> tile!");

				succeeded = false;
			}
			else
				chestTile.Replace(DungeonKeyTile.Type.Chest);
		}
		else
		{
			Debug.LogWarning("Did not spawn a dungeon key/chest pair - no potential tiles!");

			succeeded = false;
		}
	}
}
