using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	[Tooltip("The level generator profile to use when generating levels.")]
	public LevelGeneratorProfile profile;

	[Tooltip("The collision layer that tile layout colliders are on.")]
	public LayerMask layoutLayer;

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
            LevelTile startTile = startObj.GetComponent<LevelTile>();

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
			List<Transform> doors = new List<Transform>(nextTile.doors);
			doors.Remove(connectedDoor);

			//Keep running length of trail left
			trailLength--;

			if (trailLength > 0)
			{
				//Generate another tile for each door
				foreach (Transform door in doors)
				{
					GenerateTile(door, trailLength);
				}
			}
		}
	}
}
