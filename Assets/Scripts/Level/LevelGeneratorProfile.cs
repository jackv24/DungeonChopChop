using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelGen", menuName = "Data/Level Generator Profile")]
public class LevelGeneratorProfile : ScriptableObject
{
	public LevelTile startTile;

	[Space()]
	public int maxTrailLength = 5;

	[Space()]
	public List<LevelTile> tilePool = new List<LevelTile>();

	public LevelTile GetRandomTile()
	{
		int index = Random.Range(0, tilePool.Count);

		return tilePool[index];
	}
}
