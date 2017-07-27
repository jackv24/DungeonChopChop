using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	[Tooltip("The level generator profile to use when generating levels.")]
	public LevelGeneratorProfile profile;

	[Tooltip("The collision layer that tile layout colliders are on.")]
	public LayerMask layoutLayer;

	[Space()]
	[Tooltip("How close two doors need to be to be considered connected.")]
	public float maxDoorDistance = 0.1f;

    [Space()]
    [Tooltip("Seed for the level generator. Leave at 0 for random seed.")]
    public int seed = 0;

	[Header("Biomes")]
	public float townBiomeRadius = 100.0f;
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

	[Header("In-game")]
	public bool showLoadingScreenInEditor = true;
	public bool ShowLoadingScreen { get { return showLoadingScreenInEditor || !Application.isEditor; } }
	public GameObject loadingScreen;
	public ReplaceText loadingText;
	public float stageDelay = 0.1f;
	public float fadeOutTime = 0.5f;

	private List<LevelTile> generatedTiles = new List<LevelTile>();

	private void Start()
	{
		StartCoroutine("Generate");
    }

    public IEnumerator Generate()
    {
		if (ShowLoadingScreen)
		{
			if (loadingScreen)
				loadingScreen.SetActive(true);

			if (loadingText)
				loadingText.Replace("generating tiles");
		}

		if (seed != 0)
            Random.InitState(seed);

        //Delete all children
        Clear();

        bool running = true;

        int iterations = 0;

        while (running)
        {
            iterations++;

			if (iterations > profile.maxAttempts)
			{
				Clear();

				Debug.LogWarning("Level Generator exceeded maximum number of attempts. Check to make sure the max trail length allows for generation of the minimum tile amount.");

				break;
			}

            if (!profile.startTile)
            {
                Debug.LogWarning("No start tile defined in level generator profile!");

                yield return null;
            }

            //Spawn start tile
			GameObject startObj = (GameObject)Instantiate(profile.startTile.gameObject, transform);
			startObj.transform.position = transform.position;

			LevelTile startTile = startObj.GetComponent<LevelTile>();
			generatedTiles.Add(startTile);
			startTile.ReplaceDoors();
			startTile.BlockDoors();

			//Spawn a tile for every door
            foreach (Transform door in startTile.doors)
            {
                GenerateTile(door, profile.maxTrailLength);
            }

            //If there are too few tiles, delete and roll again
            if (transform.childCount <= profile.minTileAmount)
            {
                Clear();
            }
            else
                running = false;

			if (ShowLoadingScreen && loadingText)
			{
				yield return new WaitForSeconds(stageDelay);
				loadingText.Replace("connecting doors");
				yield return new WaitForSeconds(stageDelay);
			}

			//Connect all close open doors, block open doors that don't lead anywhere
			ConnectDoors();

			if (ShowLoadingScreen && loadingText)
			{
				loadingText.Replace("replacing biomes");
				yield return new WaitForSeconds(stageDelay);
			}

			ReplaceBiomes();

			if (ShowLoadingScreen && loadingText)
			{
				loadingText.Replace("placing dungeons");
				yield return new WaitForSeconds(stageDelay);
			}

			GenerateDungeons();

			if (ShowLoadingScreen && loadingText)
			{
				loadingText.Replace("merging meshes");
				yield return new WaitForSeconds(stageDelay);
			}

			//Only apply to tiles when game is running (otherwise it is an in-editor preview)
			Finish();

			if (ShowLoadingScreen && loadingText)
				loadingText.SetFallback();

			CanvasRenderer[] rends = loadingScreen.GetComponentsInChildren<CanvasRenderer>();

			float elapsedTime = 0;

			while (elapsedTime < fadeOutTime)
			{
				foreach (CanvasRenderer rend in rends)
				{
					rend.SetAlpha(1 - (elapsedTime / fadeOutTime));
				}

				yield return new WaitForEndOfFrame();
				elapsedTime += Time.deltaTime;
			}

			if (loadingScreen)
				loadingScreen.SetActive(false);
		}
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

		generatedTiles.Clear();
    }

	private void GenerateTile(Transform connectingDoor, int trailLength)
	{
		LevelTile nextTile = null;
		Transform connectedDoor = null;

		List<LevelGeneratorProfile.GeneratorTile> possibleTiles = new List<LevelGeneratorProfile.GeneratorTile>(profile.tilePool);

		while (possibleTiles.Count > 0)
		{
            LevelGeneratorProfile.GeneratorTile possibleTile = null;

            ///Get random tile with probability
            //Sort possible tiles list by probability
            possibleTiles.Sort((x, y) => x.probability.CompareTo(y.probability));

            //Get sum of all probabilities
            float maxProbability = 0;
            foreach (LevelGeneratorProfile.GeneratorTile t in possibleTiles)
                maxProbability += t.probability;

            //Generate random number up to max probability
            float num = Random.Range(0, maxProbability);

            //Get random tile using cumulative probability
            float runningProbability = 0;
            foreach(LevelGeneratorProfile.GeneratorTile t in possibleTiles)
            {
                if (num >= runningProbability)
                    possibleTile = t;

                runningProbability += t.probability;
            }

			bool success = false;

			//Spawn tile in world
			GameObject tileObj = (GameObject)Instantiate(possibleTile.tile.gameObject, transform);
			LevelTile tile = tileObj.GetComponent<LevelTile>();

			//Loop through and test all doors to connect to
			foreach (Transform door in tile.doors)
			{
				//Rotate up to three
				for (int rotationCount = 0; rotationCount < 4; rotationCount++)
				{
					//Rotate another 90 degrees every time after the first loop
					if (rotationCount > 0)
						tileObj.transform.Rotate(new Vector3(0, 90, 0));

					//Reset tile position to door
					tileObj.transform.position = connectingDoor.position;

					//Place tile relative so their doors are connected
					Vector3 offset = connectingDoor.position - door.position;

					//Offset tile so doors are connected
					tileObj.transform.position = connectingDoor.position + offset;

					//If tile is not intersecting, it has successfully found a place
					if (!tile.IsIntersecting(layoutLayer))
					{
						success = true;
						connectedDoor = door;

						break;
					}
				}

				if (success)
					break;
			}

			if(success)
			{
				nextTile = tile;
				break;
			}
			else
			{
				//Tile did not work, remove it from list of possible tiles
				possibleTiles.Remove(possibleTile);

				//Tile did not work, so delete it
				DestroyImmediate(tileObj);
			}
		}

		//If a tile was found...
		if (nextTile)
		{
			generatedTiles.Add(nextTile);

			//Replace door prefab spawners with actual doors
			nextTile.ReplaceDoors();

			//No need to check the door that was just connected
			nextTile.doors.Remove(connectedDoor);

			nextTile.BlockDoors();

			//Keep running length of trail left
			trailLength--;

			if (trailLength > 0)
			{
				//Generate another tile for each door
				foreach (Transform door in nextTile.doors)
				{
					GenerateTile(door, trailLength);
				}
			}
		}
	}

	void ConnectDoors()
	{
		//Find all doors in scene
		LevelDoor[] doors = FindObjectsOfType<LevelDoor>();

		//Keep list of doors to be deleted
		List<GameObject> emptyDoors = new List<GameObject>();

		foreach(LevelDoor doorA in doors)
		{
			LevelDoor connectedDoor = null;

			foreach(LevelDoor doorB in doors)
			{
				if (doorA == doorB)
					continue;

				//If this door is close to another door it should not be blocked
				if (Mathf.Abs((doorA.transform.position - doorB.transform.position).magnitude) <= maxDoorDistance)
					connectedDoor = doorB;
			}

			//If this door is close to another door, it should be connected
			if (connectedDoor)
			{
				doorA.SetTarget(connectedDoor);
			}
			//If this door is not close to another door
			else
			{
				LevelTile tileA = doorA.GetComponentInParent<LevelTile>();

				if (tileA.blockedDoorPrefab)
				{
					GameObject doorObjA = Instantiate(tileA.blockedDoorPrefab, tileA.transform);
					doorObjA.transform.position = doorA.transform.position;
					doorObjA.transform.rotation = doorA.transform.rotation;
				}

				//Make sure to remove door from LevelTile, so no rooms are spawned off of it
				tileA.doors.Remove(doorA.transform);

				emptyDoors.Add(doorA.gameObject);
			}
		}

		//Remove doors that have been blocked
		for (int i = 0; i < emptyDoors.Count; i++)
			DestroyImmediate(emptyDoors[i]);

		emptyDoors.Clear();
	}

    void ReplaceBiomes()
    {
        //TODO: Actually replace biomes based on position, rather than making them all one biome
        foreach(LevelTile tile in generatedTiles)
        {
			LevelTile.Biomes biome = LevelTile.Biomes.Grass;

			if ((tile.transform.position - transform.position).magnitude > townBiomeRadius)
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

	void GenerateDungeons()
	{
		for (int i = 0; i < 4; i++)
		{
			GameObject dungeonPrefab = null;
			bool top = false;
			bool right = false;

			//Firgure out which quadrant to work in
			switch(i)
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
				foreach (LevelTile tile in generatedTiles)
				{
					bool keepTop = false;
					bool keepRight = false;

					if(top)
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
					float distance = Vector3.Distance(transform.position, tile.transform.position);

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
		if(col)
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

    void Finish()
    {
		if (Application.isPlaying)
		{
			//Combine level for batching (only when playing)
			StaticBatchingUtility.Combine(gameObject);

			for (int i = 0; i < generatedTiles.Count; i++)
			{
				if (generatedTiles[i].walls)
					generatedTiles[i].walls.SetActive(false);
				else
					Debug.Log("Tile: " + generatedTiles[i].gameObject.name + " has no walls");
			}

			generatedTiles[0].SetCurrent(null);
		}
		else
		{
			//If not playing, hide all walls
			for(int i = 0; i < transform.childCount; i++)
			{
				LevelTile tile = transform.GetChild(i).gameObject.GetComponent<LevelTile>();

				if (tile)
					tile.walls.SetActive(false);
			}
		}
    }

	void OnDrawGizmosSelected()
	{
		if(transform.childCount > 0)
		Gizmos.DrawWireSphere(transform.GetChild(0).position, townBiomeRadius);
	}
}
