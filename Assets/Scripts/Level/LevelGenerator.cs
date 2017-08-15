using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	public static LevelGenerator Instance;

	public delegate void NormalEvent();
	public event NormalEvent OnGenerationStart;
	public event NormalEvent OnGenerationFinished;
	public event NormalEvent OnTileEnter;

	[Tooltip("The level generator profile to use when generating levels.")]
	public LevelGeneratorProfile profile;

	[Tooltip("The collision layer that tile layout colliders are on.")]
	public LayerMask layoutLayer;

	[Space()]
	[Tooltip("How close two doors need to be to be considered connected.")]
	public float maxDoorDistance = 0.1f;

    [Space()]
    [Tooltip("Seed for the level generator. Leave at 0 for random seed.")]
    public int overworldSeed = 0;

	[Header("In-game")]
	public bool showLoadingScreenInEditor = true;
	public bool ShowLoadingScreen { get { return showLoadingScreenInEditor || !Application.isEditor; } }
	public GameObject loadingScreen;
	public ReplaceText loadingText;
	public float stageDelay = 0.1f;
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
		if (overworldSeed == 0)
			overworldSeed = System.DateTime.Now.Millisecond;

		StartCoroutine(Generate(overworldSeed));

		showDebugMenu = Application.isEditor;
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

			text += "Seed: " + overworldSeed + "\n\n";
			text += "Current Tile: " + (currentTile ? currentTile.gameObject.name : "NULL") + "\n";

			GUI.Label(new Rect(pos, size), text);
		}
	}

	public IEnumerator Generate(int seed)
    {
		if (OnGenerationStart != null)
			OnGenerationStart();

		if (ShowLoadingScreen)
		{
			if (loadingScreen)
				loadingScreen.SetActive(true);

			if (loadingText)
				loadingText.Replace("generating tiles");

			yield return new WaitForSeconds(stageDelay);
		}

		//Initialise random with seed
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

				Debug.LogWarning("Level Generator exceeded maximum number of attempts. Check to make sure the max trail length allows for generation of the minimum tile amount, and that all generation conditions are met.");

				break;
			}

            if (!profile.startTile)
            {
                Debug.LogWarning("No start tile defined in level generator profile!");

				break;
            }

            //Spawn start tile
			GameObject startObj = (GameObject)Instantiate(profile.startTile.gameObject, transform);
			startObj.transform.position = transform.position;

			LevelTile startTile = startObj.GetComponent<LevelTile>();
			generatedTiles.Add(startTile);
			startObj.name += generatedTiles.Count;
			startTile.ReplaceDoors();
			startTile.BlockDoors();

			currentTile = startTile;

			//Spawn a tile for every door
            foreach (Transform door in startTile.doors)
            {
                GenerateTile(door, profile.maxTrailLength);
            }

			//If there are too few tiles, delete and roll again
			if (transform.childCount <= profile.minTileAmount && !profile.succeeded)
			{
				Clear();
			}
			else
			{
				running = false;

				//Assume generation will succeed until told otherwise
				profile.succeeded = true;

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
					loadingText.Replace("skinning level");
					yield return new WaitForSeconds(stageDelay);
				}

				//After level layout is generated, generate level type-specific content
				profile.Generate(this);

				if (ShowLoadingScreen && loadingText)
				{
					loadingText.Replace("merging meshes");
					yield return new WaitForSeconds(stageDelay);
				}

				//Only apply to tiles when game is running (otherwise it is an in-editor preview)
				Finish();

				//Wait for players to be spawned, then call done event
				yield return new WaitForEndOfFrame();
				if (OnGenerationFinished != null)
					OnGenerationFinished();

				if (ShowLoadingScreen && loadingText)
					loadingText.SetFallback();

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
			}

			startTile.SetCurrent(null);
		}
    }

    public void Clear()
    {
        for(int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
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

			//Loop through and test all doors to connect to
			for(int i = 0; i < tile.doors.Count; i++)
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
			generatedTiles.Add(nextTile);
			nextTile.gameObject.name += generatedTiles.Count;

			//Replace door prefab spawners with actual doors
			nextTile.ReplaceDoors();

			//No need to check the door that was just connected
			nextTile.doors.RemoveAt(connectedDoorIndex);

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

    void Finish()
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

		//Move players to tile centre
		PlayerInformation[] playerInfos = FindObjectsOfType<PlayerInformation>();

		foreach(PlayerInformation playerInfo in playerInfos)
		{
			playerInfo.gameObject.transform.position = currentTile.tileOrigin.position + Vector3.up;
		}
    }

	public void EnterTile()
	{
		if (OnTileEnter != null)
			OnTileEnter();
	}

	public void RegenerateWithProfile(LevelGeneratorProfile p, int seed)
	{
		Clear();

		profile = p;

		StartCoroutine(Generate(seed));
	}
}
