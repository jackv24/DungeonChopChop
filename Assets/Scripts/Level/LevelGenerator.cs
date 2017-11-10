using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance;

    //Random instance specific to level generator - should prevent realtime random numbers from interfering
    public static System.Random Random;

    public delegate void NormalEvent();
    public event NormalEvent OnGenerationStart;
    public event NormalEvent OnBeforeMergeMeshes;
    public event NormalEvent OnAfterSpawnChests;
    public event NormalEvent OnGenerationFinished;
    public event NormalEvent OnTileEnter;
    public event NormalEvent OnTileClear;
    public event NormalEvent OnEnemiesSpawned;

    public bool IsFinished { get { return isFinished; } }
    private bool isFinished = false;

    [Tooltip("The level generator profile to use when generating levels.")]
	public LevelGeneratorProfile profile;

	[Tooltip("The collision layer that tile layout colliders are on.")]
	public LayerMask layoutLayer;

	[Space()]
	[Tooltip("How close two doors need to be to be considered connected.")]
	public float maxDoorDistance = 0.1f;

    [Space()]
    [Tooltip("Seed for the level generator. Leave at 0 for random seed.")]
    public int startSeed = 0;
    private int lastSeed;
	public int LastSeed { get { return lastSeed; } }

    [Header("In-game")]
	public bool showLoadingScreenInEditor = true;
	public bool ShowLoadingScreen { get { return showLoadingScreenInEditor || !Application.isEditor; } }
    public bool useKnownSeeds = false;
    public bool loopRegeneration = false;

    [HideInInspector]
	public List<LevelTile> generatedTiles = new List<LevelTile>();
	[HideInInspector]
	public LevelTile currentTile;

    private int tileUpdatePause;
    private int tilesUpdated = 0;

    void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
        //If seed is zero, set seed with "arbitrary" value
        if (Application.isEditor && !useKnownSeeds)
        {
            if (startSeed == 0)
                startSeed = System.DateTime.Now.Millisecond;
        }
		else
		{
            startSeed = SaveManager.GetSeed();

			if (startSeed == 0)
                startSeed = System.DateTime.Now.Millisecond;
        }

		if(!Application.isEditor)
            loopRegeneration = false;

        tileUpdatePause = LoadingScreen.TileUpdatePause;

        StartCoroutine(Generate(startSeed));
    }

	void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	public IEnumerator Generate(int seed)
    {
		lastSeed = seed;
		Debug.Log("Starting generation with seed: " + seed);

		Random = new System.Random(seed);

        if (profile is OverworldGeneratorProfile)
        {
            if (!SaveManager.CheckSeed(seed))
            {
                while (true)
                {
                    seed = Random.Next(0, 1000);
                    lastSeed = seed;

                    if (SaveManager.CheckSeed(seed))
                        break;
                }
            }
        }

        if (OnGenerationStart != null)
			OnGenerationStart();

        LoadingScreen.Show("Generating Level", false);

        int iterations = 0;

		profile.succeeded = false;
        isFinished = false;

        while (loopRegeneration || (iterations < profile.maxAttempts && !profile.succeeded))
        {
			Clear();
			yield return new WaitForEndOfFrame();

			//If first seed did not work, try another
			if (iterations > 0)
			{
				if(profile is OverworldGeneratorProfile && !profile.succeeded)
                	SaveManager.RegisterFailedSeed(seed);

                while (true)
                {
                    seed = Random.Next(0, 1000);
					lastSeed = seed;

                    if (profile is OverworldGeneratorProfile)
                    {
                        if (SaveManager.CheckSeed(seed))
                            break;
                    }
					else
                        break;
                }

                Debug.LogWarning("Generation did not succeed, trying seed: " + seed);
			}

            //Assume succeeded until set otherwise
            profile.succeeded = true;

			Random = new System.Random(seed);

			iterations++;

            if (!profile.startTile)
            {
                Debug.LogWarning("No start tile defined in level generator profile!");

				break;
            }

            //Spawn start tile
			GameObject startObj = (GameObject)Instantiate(profile.startTile.gameObject, transform);
			startObj.transform.position = transform.position;

            //Make sure tile is active to prevent errors if it's accidentally disabled
            if (!startObj.activeSelf)
            {
                Debug.LogWarning(startObj.name + "Spawned Inactive!");
                startObj.SetActive(true);
            }

            LevelTile startTile = startObj.GetComponent<LevelTile>();
			startTile.index = generatedTiles.Count;
			generatedTiles.Add(startTile);
			startObj.name += generatedTiles.Count;
			startTile.ReplaceDoors();
			startTile.BlockDoors(-1);

			currentTile = startTile;

			//Spawn a tile for every door
            foreach (Transform door in startTile.doors)
            {
                yield return GenerateTile(door, profile.maxTrailLength);
            }

			//If there are too few tiles, roll again
			if (transform.childCount <= profile.minTileAmount && !profile.succeeded)
			{
				continue;
			}

			yield return new WaitForEndOfFrame();

			//Connect all close open doors, block open doors that don't lead anywhere
			ConnectDoors();

			yield return new WaitForEndOfFrame();

			//After level layout is generated, generate level type-specific content
			profile.Generate(this);

			if(profile.succeeded && profile is OverworldGeneratorProfile)
                SaveManager.RegisterSucceededSeed(seed);

            startTile.SetCurrent(null);
		}

		yield return new WaitForEndOfFrame();

		if (OnBeforeMergeMeshes != null)
			OnBeforeMergeMeshes();

		yield return new WaitForEndOfFrame();
		//Merge meshes etc when level is finished generating
		Finish();

		//Spawn chests AFTER meshes are merged
		SpawnChests();

		if (OnAfterSpawnChests != null)
			OnAfterSpawnChests();

		//Wait for players to be spawned, then call done event
		yield return new WaitForEndOfFrame();
        if (OnGenerationFinished != null)
            OnGenerationFinished();

        isFinished = true;

		yield return new WaitForEndOfFrame();

		if (iterations >= profile.maxAttempts)
		{
			Debug.LogWarning("Level Generator exceeded maximum number of attempts. Check to make sure the max trail length allows for generation of the minimum tile amount, and that all generation conditions are met.");
		}

        LoadingScreen.Hide();
    }

    public void Clear()
    {
		int childAmount = transform.childCount;

		for (int i = 0; i < childAmount; i++)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}

		generatedTiles.Clear();
    }

	private IEnumerator GenerateTile(Transform connectingDoor, int trailLength)
	{
		if(tileUpdatePause > 0)
		{
            tilesUpdated++;

			if(tilesUpdated >= tileUpdatePause)
			{
                tilesUpdated = 0;

                yield return new WaitForEndOfFrame();
            }
        }

		LevelTile nextTile = null;
		int connectedDoorIndex = -1;

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
            float num = Random.NextFloat(0, maxProbability);

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

            //Make sure tile is active to prevent errors if it's accidentally disabled
            if (!tileObj.activeSelf)
            {
                Debug.LogWarning(tileObj.name + "Spawned Inactive!");
                tileObj.SetActive(true);
            }

			LevelTile tile = tileObj.GetComponent<LevelTile>();

			int rotation = 0;

			//Loop through and test all doors to connect to
			for(int i = 0; i < tile.doors.Count; i++)
			{
				//Rotate up to three
				for (int rotationCount = 0; rotationCount < 4; rotationCount++)
				{
					//Rotate another 90 degrees every time after the first loop
					if (rotationCount > 0)
						rotation += 90;
					else
						rotation = 0;

					//Limit tile rotation
					if ((rotation == 90 || rotation == 270) && tile.limitToHorizontal)
					{
						continue;
					}

					tileObj.transform.eulerAngles = new Vector3(0, rotation, 0);

					//Reset tile position to door
					tileObj.transform.position = connectingDoor.position;

					//Place tile relative so their doors are connected
					Vector3 offset = connectingDoor.position - tile.doors[i].position;

					//Offset tile so doors are connected
					tileObj.transform.position = connectingDoor.position + offset;

					//If tile is not intersecting, it has successfully found a place
					if (!tile.IsIntersecting(layoutLayer))
					{
						success = true;
						connectedDoorIndex = i;

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
			nextTile.index = generatedTiles.Count;
			generatedTiles.Add(nextTile);
			nextTile.gameObject.name += generatedTiles.Count;

			//Replace door prefab spawners with actual doors
			nextTile.ReplaceDoors();

			nextTile.BlockDoors(connectedDoorIndex);

			//Keep running length of trail left
			trailLength--;

			if (trailLength > 0)
			{
				//Generate another tile for each door
				foreach (Transform door in nextTile.doors)
				{
					if (connectedDoorIndex > nextTile.doors.Count - 1)
						Debug.Log(connectedDoorIndex + ", " + nextTile.doors.Count);

					//No need to check the door that was just connected
					if (door == nextTile.doors[connectedDoorIndex])
						continue;

					yield return GenerateTile(door, trailLength);
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

    void Finish()
    {
		//Bake tile navmeshes (before batching to allow mesh read access)
		for (int i = 0; i < generatedTiles.Count; i++)
		{
			NavMeshSurface surface = generatedTiles[i].GetComponentInChildren<NavMeshSurface>();

			if (surface)
				surface.BuildNavMesh();
		}

		//Combine level for batching
		StaticBatchingUtility.Combine(gameObject);

		//Move players to tile centre
		PlayerInformation[] playerInfos = FindObjectsOfType<PlayerInformation>();

		//Move players to origin of current tile
		foreach(PlayerInformation playerInfo in playerInfos)
		{
			playerInfo.gameObject.transform.position = currentTile.tileOrigin.position + Vector3.up;
		}

		//Show already visited dungeons
		if (LevelVars.Instance)
		{
			for (int i = 0; i < generatedTiles.Count; i++)
			{
				if (!LevelVars.Instance.levelData.inDungeon && LevelVars.Instance.levelData.clearedTiles.Contains(generatedTiles[i].index))
					generatedTiles[i].ShowTile(false, true);
				else
					generatedTiles[i].ShowTile(false, true, false);
			}
		}

		generatedTiles[0].SetCurrent(null);
	}

	void SpawnChests()
	{
		//Find all chest spawns
		ChestSpawn[] spawns = FindObjectsOfType<ChestSpawn>();

		List<ChestSpawn> chestsSpawned = new List<ChestSpawn>();
		List<ChestSpawn> chestsNotSpawned = new List<ChestSpawn>();

		//loop through all spawns
		for (int i = 0; i < spawns.Length; i++)
		{
			bool shouldSpawn = false;

			//Make sure something is spawned at all dead ends
			LevelTile parentTile = spawns[i].GetComponentInParent<LevelTile>();

			if (spawns[i].spawnType == ChestSpawn.SpawnType.AlwaysSpawn)
				shouldSpawn = true;
			else if (spawns[i].spawnType == ChestSpawn.SpawnType.Probability)
			{
				//If not a dead end, spawn based on probability
				float value = Random.NextFloat(0, 1f);
				if (value <= profile.chestSpawnProbability)
					shouldSpawn = true;
			}
			else if (spawns[i].spawnType == ChestSpawn.SpawnType.ClearRoom)
				spawns[i].SetSpawnOnClear(parentTile);

			if(shouldSpawn)
			{
				if (spawns[i].Spawn())
					chestsSpawned.Add(spawns[i]);
				else
					chestsNotSpawned.Add(spawns[i]);
			}
		}
	}

	public void EnterTile()
	{
		if (OnTileEnter != null)
			OnTileEnter();
	}

	public void ClearTile()
	{
		if (OnTileClear != null)
			OnTileClear();
	}

	public void EnemiesSpawned()
	{
		if(OnEnemiesSpawned != null)
            OnEnemiesSpawned();
    }

	public void RegenerateWithProfile(LevelGeneratorProfile p, int seed)
	{
		RegenerateWithProfile(p, seed, Vector3.zero, -1);
	}
	public void RegenerateWithProfile(LevelGeneratorProfile p, int seed, Vector3 position, int tileIndex)
	{
		if (currentTile)
		{
			TileParticles particles = currentTile.GetComponent<TileParticles>();

			if (particles)
				particles.OnExit();
		}

        isFinished = false;

        Clear();
        ObjectPooler.ReturnAll();

        profile = p;

		StartCoroutine(Generate(seed));

		NormalEvent handler = null;
		handler = () =>
		{
			PlayerInformation[] infos = FindObjectsOfType<PlayerInformation>();

			foreach (PlayerInformation info in infos)
				info.gameObject.transform.position = position;

			if(tileIndex >= 0)
			{
				currentTile = generatedTiles[tileIndex];
				currentTile.SetCurrent(null);
			}

			OnGenerationFinished -= handler;
		};
		OnGenerationFinished += handler;
	}
}
