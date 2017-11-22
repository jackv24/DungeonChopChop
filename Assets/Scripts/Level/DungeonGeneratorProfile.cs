﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon", menuName = "Data/Dungeon Generator Profile")]
public class DungeonGeneratorProfile : LevelGeneratorProfile
{
    public LevelTile.Biomes dungeonBiome;

    [HideInInspector] public GameObject keyTileObj;
    [HideInInspector] public GameObject chestTileObj;

	[HideInInspector]
	public GameObject bossTile;

	[HideInInspector]
	public GameObject keyTilePrefab;
	[HideInInspector]
	public GameObject chestTilePrefab;

	public override void Generate(LevelGenerator levelGenerator)
	{
		if(dungeonBiome != LevelTile.Biomes.Dungeon1
		&& dungeonBiome != LevelTile.Biomes.Dungeon2
		&& dungeonBiome != LevelTile.Biomes.Dungeon3
		&& dungeonBiome != LevelTile.Biomes.Dungeon4
		&& dungeonBiome != LevelTile.Biomes.BossDungeon)
		{
            dungeonBiome = LevelTile.Biomes.Dungeon1;

            Debug.LogWarning("Dungeon Generator Profile does not have a dungeon biome selected, defaulting to dungeon 1");
        }

		//Replace layout tiles with dungeon tiles
		foreach(LevelTile tile in levelGenerator.generatedTiles)
		{
			tile.Replace(dungeonBiome);
		}

		//Get the biome that this dungeon was entered from
		LevelTile.Biomes biome = LevelVars.Instance.lastOverworldBiome;

		if (dungeonBiome != LevelTile.Biomes.BossDungeon)
		{
			///Spawn dungeon key and chest
			//Keep list of potential places to spawn
			List<DungeonKeyTile> potentialTiles = new List<DungeonKeyTile>();

			//Get tiles that can be replaced from level generator
			foreach (LevelTile tile in levelGenerator.generatedTiles)
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

				float furthestDistance = 0;

				//Key tile should be furthest from the entrance
				foreach (DungeonKeyTile tile in potentialTiles)
				{
					float distance = Vector3.Distance(tile.transform.position, levelGenerator.generatedTiles[0].transform.position);

					if (distance > furthestDistance)
					{
						furthestDistance = distance;

						keyTile = tile;
					}
				}

				if (keyTile)
				{
					//Key tile should not be considered for chest tile
					potentialTiles.Remove(keyTile);

					furthestDistance = 0;
					//Chest tile should be furthest away from key tile
					foreach (DungeonKeyTile tile in potentialTiles)
					{
						float distance = Vector3.Distance(tile.transform.position, keyTile.transform.position);

						if (distance > furthestDistance)
						{
							furthestDistance = distance;

							chestTile = tile;
						}
					}
				}

				//Make sure there is a key/chest pair
				if (!keyTile)
				{
					Debug.LogWarning("Did not spawn a dungeon <b>key</b> tile!");

					succeeded = false;
				}
				else
				{
					keyTileObj = keyTile.Replace(keyTilePrefab);
				}

				if (!chestTile)
				{
					Debug.LogWarning("Did not spawn a dungeon <b>chest</b> tile!");

					succeeded = false;
				}
				else
					chestTileObj = chestTile.Replace(chestTilePrefab);
			}
			else
			{
				Debug.LogWarning("Did not spawn a dungeon key/chest pair - no potential tiles!");

				succeeded = false;
			}
		}
		else if(bossTile)
		{
			Debug.LogError("<b>Boss tile generation NOT IMPLEMENTED</b>");
		}

		//Append biome to all persistent object identifiers, since each dungeon should be considered different
		foreach(LevelTile tile in levelGenerator.generatedTiles)
		{
            PersistentObject[] objs = tile.GetComponentsInChildren<PersistentObject>();

			foreach(PersistentObject obj in objs)
			{
                if (!obj.onStart)
                {
                    obj.identifier += "_" + biome.ToString();
                    obj.Setup();
                }
            }
        }
	}
}
