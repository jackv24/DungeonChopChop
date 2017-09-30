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
	public Sprite chestSpawnerIcon;

	[Space()]
	public LayerMask groundLayer;

	[System.Serializable]
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
	[Header("Used by Level Generator")]
	public LevelData levelData = new LevelData();
	public LevelTile.Biomes lastOverworldBiome;

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

			//Show icon for unspawned chests, event run once
			LevelGenerator.NormalEvent tempEvent = null;
			tempEvent = delegate
			{
				ChestSpawn[] spawns = FindObjectsOfType<ChestSpawn>();

				foreach(ChestSpawn spawn in spawns)
				{
					if(spawn.spawnType == ChestSpawn.SpawnType.ClearRoom)
					{
						MapTracker tracker = spawn.gameObject.AddComponent<MapTracker>();
						tracker.sprite = chestSpawnerIcon;
						tracker.showOnTileEnter = true;
						tracker.registerOnce = true;
						tracker.Setup();

						spawn.OnChestSpawned += tracker.Remove;
					}
				}

				LevelGenerator.Instance.OnAfterSpawnChests -= tempEvent;
			};
			LevelGenerator.Instance.OnAfterSpawnChests += tempEvent;
		}
	}

	void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}
}
