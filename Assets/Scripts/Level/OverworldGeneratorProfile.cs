using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Overworld", menuName = "Data/Overworld Generator Profile")]
public class OverworldGeneratorProfile : LevelGeneratorProfile
{
	[Header("Biomes")]
	public float townBiomeRadius = 60.0f;
	public LevelTile.Biomes townBiome = LevelTile.Biomes.Grass;

	[HideInInspector] public LevelTile.Biomes topRightBiome;
	[HideInInspector] public LevelTile.Biomes bottomRightBiome;
	[HideInInspector] public LevelTile.Biomes bottomLeftBiome;
	[HideInInspector] public LevelTile.Biomes topLeftBiome;

	public override void Generate(LevelGenerator levelGenerator)
	{
		RandomiseBiomes();
		ReplaceBiomes(levelGenerator);

		GenerateDungeons(levelGenerator);

		//Only bother to continue generating if it hasn't failed already
		if(levelGenerator.profile.succeeded != false)
        	GenerateSpecialTiles(levelGenerator);
    }

	void RandomiseBiomes()
	{
		List<LevelTile.Biomes> unusedBiomes = new List<LevelTile.Biomes>();

		//Add the four overworld biomes to list (excluding town biome and dungeons)
		unusedBiomes.Add(LevelTile.Biomes.Desert);
		unusedBiomes.Add(LevelTile.Biomes.Forest);
		unusedBiomes.Add(LevelTile.Biomes.Ice);
		unusedBiomes.Add(LevelTile.Biomes.Fire);

		for(int i = 0; i < 4; i++)
		{
			//Get random biome and remove from list (only use once)
			LevelTile.Biomes biome = unusedBiomes[LevelGenerator.Random.Next(0, unusedBiomes.Count)];
			unusedBiomes.Remove(biome);

			//Set one of 4 directions tot his biome
			switch(i)
			{
				case 0:
					topRightBiome = biome;
					break;
				case 1:
					bottomRightBiome = biome;
					break;
				case 2:
					bottomLeftBiome = biome;
					break;
				case 3:
					topLeftBiome = biome;
					break;
			}
		}
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

			int seed = LevelGenerator.Random.Next(0, 1000);

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
				if (tile.Biome == biome && tile.currentGraphic)
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

	void GenerateSpecialTiles(LevelGenerator levelGenerator)
	{
        LevelTile.Biomes[] biomes = { LevelTile.Biomes.Forest, LevelTile.Biomes.Ice, LevelTile.Biomes.Desert, LevelTile.Biomes.Fire };

        foreach (LevelTile.Biomes biome in biomes)
        {
            bool[] hasPlaced = new bool[System.Enum.GetNames(typeof(SpecialTile.SpecialType)).Length];

            for (int i = 0; i < hasPlaced.Length; i++)
                hasPlaced[i] = false;

            List<SpecialTile> possibleTiles = new List<SpecialTile>();

            foreach (LevelTile tile in levelGenerator.generatedTiles)
            {
				if(tile.Biome != biome)
                    continue;

                SpecialTile special = tile.GetComponent<SpecialTile>();

                if (special)
                    possibleTiles.Add(special);
            }

            for (int i = 0; i < hasPlaced.Length; i++)
			{
                List<SpecialTile> tryTiles = new List<SpecialTile>(possibleTiles);

				while(tryTiles.Count > 0)
				{
                    SpecialTile tile = tryTiles[LevelGenerator.Random.Next(0, tryTiles.Count)];

                    hasPlaced[i] = tile.Replace((SpecialTile.SpecialType)i);

					if(hasPlaced[i])
                        break;

                    tryTiles.Remove(tile);
                }
            }

            for (int i = 0; i < hasPlaced.Length; i++)
			{
				if(!hasPlaced[i])
				{
                    //levelGenerator.profile.succeeded = false;

                    Debug.LogWarning("Level Generator failed to place a " + ((SpecialTile.SpecialType)i).ToString() + " special tile for biome " + biome.ToString());
                }
			}
        }
    }
}
