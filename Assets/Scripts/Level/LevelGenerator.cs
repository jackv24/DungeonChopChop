using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	public LevelGeneratorProfile profile;

	private void Start()
	{
		GameObject startObj = (GameObject)Instantiate(profile.startTile.gameObject, transform);

		LevelTile startTile = startObj.GetComponent<LevelTile>();

		foreach (Transform door in startTile.doors)
		{
			GenerateTile(door, profile.maxTrailLength);
		}
	}

	private void GenerateTile(Transform connectingDoor, int trailLength)
	{
		LevelTile nextTile = null;

		while(nextTile == null)
		{
			//Find a suitable tile (currently randomly selects tile)

			LevelTile possibleTile = profile.GetRandomTile();

			if (possibleTile == null)
				return;
			else
			{
				GameObject tileObj = (GameObject)Instantiate(possibleTile.gameObject, transform);
				LevelTile tile = tileObj.GetComponent<LevelTile>();

				Transform door = tile.GetRandomDoor();

				Vector3 offset = door.position - connectingDoor.position;

				tileObj.transform.position = connectingDoor.position + offset;

				//TODO: Check if overlapping, attempt rotation, check again, if full rotation doesn't work choose another tile.

				nextTile = tile;
			}
		}

		trailLength--;

		if (trailLength > 0)
		{
			foreach (Transform door in nextTile.doors)
			{
				GenerateTile(door, trailLength);
			}
		}
	}
}
