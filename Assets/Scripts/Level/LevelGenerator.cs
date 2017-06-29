using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	[Tooltip("The level generator profile to use when generating levels.")]
	public LevelGeneratorProfile profile;

	[Tooltip("The collision layer that tile layout colliders are on.")]
	public LayerMask layoutLayer;

	[Header("Debugging")]
	public bool showProcess = false;

	private void Start()
	{
		StartCoroutine("Generator");
	}

	private IEnumerator Generator()
	{
		GameObject startObj = (GameObject)Instantiate(profile.startTile.gameObject, transform);

		LevelTile startTile = startObj.GetComponent<LevelTile>();

		foreach (Transform door in startTile.doors)
		{
			yield return StartCoroutine(GenerateTile(door, profile.maxTrailLength));
		}
	}

	private IEnumerator GenerateTile(Transform connectingDoor, int trailLength)
	{
		LevelTile nextTile = null;

		List<LevelTile> possibleTiles = new List<LevelTile>(profile.tilePool);

		while (possibleTiles.Count > 0)
		{
			//Get random tile
			LevelTile possibleTile = possibleTiles[Random.Range(0, possibleTiles.Count)];

			bool success = false;

			//Spawn tile in world
			GameObject tileObj = (GameObject)Instantiate(possibleTile.gameObject, transform);
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
						break;
					}

					if(showProcess)
						yield return new WaitForEndOfFrame();
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
				Destroy(tileObj);

				if(showProcess)
					yield return new WaitForEndOfFrame();
			}
		}

		//If a tile was found...
		if (nextTile)
		{
			//Keep running length of trail left
			trailLength--;

			if (trailLength > 0)
			{
				//Generate another tile for each door
				foreach (Transform door in nextTile.doors)
				{
					yield return GenerateTile(door, trailLength);
				}
			}
		}
	}
}
