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
	public GameObject lockedChestPrefab;
	public GameObject dungeonChestPrefab;

	[Header("Biome Particles")]
	public GameObject grassParticles;
	public GameObject forestParticles;
	public GameObject desertParticles;
	public GameObject iceParticles;
	public GameObject fireParticles;
	public GameObject dungeonParticles;

	[Header("Misc")]
	public GameObject droppedCharmPrefab;

	[Space()]
	public LayerMask groundLayer;

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

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		if(LevelGenerator.Instance)
		{
			levelData.overworldProfile = LevelGenerator.Instance.profile;
			levelData.overworldSeed = LevelGenerator.Instance.startSeed;
		}
	}

	void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}
}
