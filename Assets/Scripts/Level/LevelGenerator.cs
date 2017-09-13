using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
	public static LevelGenerator Instance;

	public delegate void NormalEvent();
	public event NormalEvent OnGenerationStart;
	public event NormalEvent OnBeforeMergeMeshes;
	public event NormalEvent OnGenerationFinished;
	public event NormalEvent OnTileEnter;
	public event NormalEvent OnTileClear;

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

	[Header("In-game")]
	public bool showLoadingScreenInEditor = true;
	public bool ShowLoadingScreen { get { return showLoadingScreenInEditor || !Application.isEditor; } }
	public GameObject loadingScreen;
	public ReplaceText loadingText;
	public float fadeOutTime = 0.5f;

	[HideInInspector]
	public List<LevelTile> generatedTiles = new List<LevelTile>();
	[HideInInspector]
	public LevelTile currentTile;

	[Space()]
	public bool showDebugMenu = false;

	void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		//If seed is zero, set seed with "arbitrary" value
		if (startSeed == 0)
			startSeed = System.DateTime.Now.Millisecond;

		StartCoroutine(Generate(startSeed));

		showDebugMenu = Application.isEditor;
    }

	void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
			showDebugMenu = !showDebugMenu;
	}

	private void OnGUI()
	{
		if (showDebugMenu)
		{
			Vector2 size = new Vector2(Screen.width - 10, 400);
			Vector2 pos = new Vector2(10, Screen.height - size.y);

			string text = "<b>Debug Menu:</b>\n";

			text += "Start Seed: " + startSeed + "\n\n";
			text += "Current Tile: " + (currentTile ? currentTile.gameObject.name : "NULL") + "\n";

			GUI.Label(new Rect(pos, size), text);
		}
	}

	public IEnumerator Generate(int seed)
    {
		if (OnGenerationStart != null)
			OnGenerationStart();

		if (ShowLoadingScreen && loadingScreen)
			loadingScreen.SetActive(true);

        int iterations = 0;

		profile.succeeded = false;

		while (iterations < profile.maxAttempts && !profile.succeeded)
        {
			//Assume succeeded until set otherwise
			profile.succeeded = true;

			Clear();
			yield return new WaitForEndOfFrame();

			//If first seed did not work, try another
			if (iterations > 0)
			{
				seed = Random.Range(0, 1000);

				Debug.LogWarning("Generation did not succeed, trying seed: " + seed);
			}

			Random.InitState(seed);

			iterations++;

            if (!profile.startTile)
            {
                Debug.LogWarning("No start tile defined in level generator profile!");

				break;
            }

            //Spawn start tile
			GameObject startObj = (GameObject)Instantiate(profile.startTile.gameObject, transform);
			startObj.transform.position = transform.position;

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
                GenerateTile(door, profile.maxTrailLength);
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

		//Wait for players to be spawned, then call done event
		yield return new WaitForEndOfFrame();
		if (OnGenerationFinished != null)
			OnGenerationFinished();

		if (ShowLoadingScreen && loadingText)
			loadingText.SetFallback();
		yield return new WaitForEndOfFrame();

		if (loadingScreen)
		{
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

			loadingScreen.SetActive(false);
		}

		if (iterations >= profile.maxAttempts)
		{
			Debug.LogWarning("Level Generator exceeded maximum number of attempts. Check to make sure the max trail length allows for generation of the minimum tile amount, and that all generation conditions are met.");
		}
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

	private void GenerateTile(Transform connectingDoor, int trailLength)
	{
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
					generatedTiles[i].ShowTile(false);
				else
					generatedTiles[i].ShowTile(false, false);
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
				float value = Random.Range(0, 1f);
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

	public void RegenerateWithProfile(LevelGeneratorProfile p, int seed)
	{
		Clear();

		profile = p;

		StartCoroutine(Generate(seed));
	}
}
