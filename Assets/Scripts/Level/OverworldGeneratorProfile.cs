using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Overworld", menuName = "Data/Overworld Generator Profile")]
public class OverworldGeneratorProfile : LevelGeneratorProfile
{
	[Header("Biomes")]
	public float townBiomeRadius = 60.0f;
	public LevelTile.Biomes townBiome;
	[Space()]
	public LevelTile.Biomes topRightBiome;
	public LevelTile.Biomes bottomRightBiome;
	public LevelTile.Biomes bottomLeftBiome;
	public LevelTile.Biomes topLeftBiome;

	public override void Generate(LevelGenerator levelGenerator)
	{
		ReplaceBiomes(levelGenerator);

		GenerateDungeons(levelGenerator);
	}

	void ReplaceBiomes(LevelGenerator levelGenerator)
	{
		//TODO: Actually replace biomes based on position, rather than making them all one biome
		foreach (LevelTile tile in levelGenerator.generatedTiles)
		{
			LevelTile.Biomes biome = LevelTile.Biomes.Grass;

			if ((tile.transform.position - levelGenerator.transform.position).magnitude > townBiomeRadius)
			{
				if (tile.transform.position.x < 0 && tile.transform.position.z > 0)
					biome = topLeftBiome;
				else if (tile.transform.position.x < 0 && tile.transform.position.z < 0)
					biome = bottomLeftBiome;
				else if (tile.transform.position.x > 0 && tile.transform.position.z < 0)
					biome = bottomRightBiome;
				else if (tile.transform.position.x > 0 && tile.transform.position.z > 0)
					biome = topRightBiome;
			}

			tile.Replace(biome);
		}

		//Make sure there are no mini-biomes inside other biomes
		foreach(LevelTile tile in levelGenerator.generatedTiles)
		{
			tile.EnsureBiomeContinuity();
		}
	}

	void GenerateDungeons(LevelGenerator levelGenerator)
	{
		int dungeonCount = 0;

		for (int i = 0; i < 4; i++)
		{
			LevelTile.Biomes biome = LevelTile.Biomes.Grass;

			int seed = Random.Range(0, 1000);

			//Figure out which quadrant to work in
			switch (i)
			{
				case 0:
					biome = LevelTile.Biomes.Forest;
					break;
				case 1:
					biome = LevelTile.Biomes.Ice;
					break;
				case 2:
					biome = LevelTile.Biomes.Desert;
					break;
				case 3:
					biome = LevelTile.Biomes.Fire;
					break;
			}

			List<DungeonTile> replaceTiles = new List<DungeonTile>();

			//Get all tiles in this quadrant
			foreach (LevelTile tile in levelGenerator.generatedTiles)
			{
				if (tile.Biome == biome)
				{
					DungeonTile dungeon = tile.currentGraphic.GetComponent<DungeonTile>();

					if (dungeon)
						replaceTiles.Add(dungeon);
				}
			}

			//Keep track of furthest tile and it's distance
			DungeonTile furthestTile = null;
			float furthestDistance = 0;

			//Find furthest tile
			foreach (DungeonTile tile in replaceTiles)
			{
				float distance = Vector3.Distance(levelGenerator.transform.position, tile.transform.position);

				if (distance > furthestDistance)
				{
					furthestTile = tile;
					furthestDistance = distance;
				}
			}

			//If a furthest tile was found...
			if (furthestTile)
			{
				//Replace dungeon tile
				GameObject obj = furthestTile.Replace();

				dungeonCount++;

				DungeonEntrance entrance = obj.GetComponentInChildren<DungeonEntrance>();

				if (entrance)
					entrance.seed = seed;
			}
		}

		//Generation did not succed if not enough dungeons
		if (dungeonCount < 4)
		{
			succeeded = false;

			Debug.LogWarning("Level Generator did not generate enough dungeons! Amount: " + dungeonCount);
		}
	}
}
