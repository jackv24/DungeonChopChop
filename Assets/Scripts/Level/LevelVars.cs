using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelVars : MonoBehaviour
{
	public static LevelVars Instance;

	[Header("Enemy Spawning")]
	public GameObject enemySpawnEffect;
	public float enemySpawnDelay = 0.5f;

	[Header("Chest Spawning")]
	public GameObject normalChestPrefab;
	public GameObject specialChestPrefab;

	[Header("Misc")]
	public GameObject droppedItemPrefab;

	public class LevelData
	{
		public LevelGeneratorProfile overworldProfile;
		public int overworldSeed = 0;

		public bool inDungeon = false;

		public List<int> clearedTiles = new List<int>();

		public void SetClearedTile(int index)
		{
			//Only track visited tiles in overworld
			if(!inDungeon)
			{
				if(!clearedTiles.Contains(index))
					clearedTiles.Add(index);
			}
		}
	}
	public LevelData levelData = new LevelData();

	void Start()
	{
		if(LevelGenerator.Instance)
		{
			levelData.overworldProfile = LevelGenerator.Instance.profile;
			levelData.overworldSeed = LevelGenerator.Instance.startSeed;
		}
	}

	private void Awake()
	{
		Instance = this;
	}
}
