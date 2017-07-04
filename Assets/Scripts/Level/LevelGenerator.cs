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

	private void Start()
	{
		Generate();
    }

    public void Generate()
    {
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

			//Spawn start tile
			GameObject startObj = (GameObject)Instantiate(profile.startTile.gameObject, transform);
			startObj.transform.position = transform.position;

			LevelTile startTile = startObj.GetComponent<LevelTile>();

			startTile.BlockDoors();
			startTile.EnableStaticBatching();

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

			ConnectDoors();
        }
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
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
            //No need to check the door that was just connected
			nextTile.doors.Remove(connectedDoor);

			nextTile.BlockDoors();
			nextTile.EnableStaticBatching();

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
}
