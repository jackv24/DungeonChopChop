using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon", menuName = "Data/Dungeon Generator Profile")]
public class DungeonGeneratorProfile : LevelGeneratorProfile
{
	public override void Generate(LevelGenerator levelGenerator)
	{
		Debug.LogWarning("Dungeon generator incomplete!");
	}
}
