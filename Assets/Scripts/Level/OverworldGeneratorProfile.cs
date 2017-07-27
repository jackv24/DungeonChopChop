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

	[Header("Map Features")]
	public GameObject topRightDungeon;
	public GameObject bottomRightDungeon;
	public GameObject bottomLeftDungeon;
	public GameObject topLeftDungeon;
	[Space()]
	public float lockDungeonAngle = 45.0f;

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
	}

	void GenerateDungeons(LevelGenerator levelGenerator)
	{
		for (int i = 0; i < 4; i++)
		{
			GameObject dungeonPrefab = null;
			bool top = false;
			bool right = false;

			//Firgure out which quadrant to work in
			switch (i)
			{
				case 0:
					dungeonPrefab = topRightDungeon;
					top = true;
					right = true;
					break;
				case 1:
					dungeonPrefab = bottomRightDungeon;
					top = false;
					right = true;
					break;
				case 2:
					dungeonPrefab = bottomLeftDungeon;
					top = false;
					right = false;
					break;
				case 3:
					dungeonPrefab = topLeftDungeon;
					top = true;
					right = false;
					break;
			}

			//If there is a dungeon prefab for this quadrant
			if (dungeonPrefab)
			{
				List<LevelTile> potentialTiles = new List<LevelTile>();

				//Get all tiles in this quadrant
				foreach (LevelTile tile in levelGenerator.generatedTiles)
				{
					bool keepTop = false;
					bool keepRight = false;

					if (top)
					{
						if (tile.transform.position.z > 0)
							keepTop = true;
					}
					else
					{
						if (tile.transform.position.z < 0)
							keepTop = true;
					}

					if (right)
					{
						if (tile.transform.position.x > 0)
							keepRight = true;
					}
					else
					{
						if (tile.transform.position.x < 0)
							keepRight = true;
					}

					if (keepTop && keepRight)
						potentialTiles.Add(tile);
				}

				//Keep track of furthest tile and it's distance
				LevelTile furthestTile = null;
				float furthestDistance = 0;

				//Find furthest tile
				foreach (LevelTile tile in potentialTiles)
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
					//Place dungeon withi tile
					PlaceDungeon(dungeonPrefab, furthestTile);
				}
			}
		}
	}

	void PlaceDungeon(GameObject dungeonPrefab, LevelTile tile)
	{
		//Instantiate a dungeon
		GameObject dungeonObj = Instantiate(dungeonPrefab, tile.transform);
		dungeonObj.transform.localPosition = Vector3.zero;

		Collider col = dungeonObj.GetComponent<Collider>();

		//Place dungeon withing tile
		if (col)
			dungeonObj.transform.position = tile.GetPosInTile(col.bounds.size.x, col.bounds.size.z);
		else
			Debug.LogWarning("BoxCollider not found on dungeon in tile: " + tile.gameObject.name);

		//Rotate dungeon to face towards tile centre
		Vector3 lookPos = tile.tileOrigin.position - dungeonObj.transform.position;
		lookPos.y = 0;

		if (lookPos != Vector3.zero)
		{
			Quaternion rotation = Quaternion.LookRotation(lookPos);
			Vector3 euler = rotation.eulerAngles;
			euler.y = Mathf.Round(euler.y / lockDungeonAngle) * lockDungeonAngle;
			rotation.eulerAngles = euler;

			dungeonObj.transform.rotation = rotation;
		}
	}
}
