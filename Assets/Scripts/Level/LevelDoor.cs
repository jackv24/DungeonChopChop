using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
	public LevelTile targetTile;
	public LevelDoor targetDoor;

	public void SetTarget(LevelDoor target)
	{
		targetDoor = target;

		targetTile = target.GetComponentInParent<LevelTile>();
	}
}
