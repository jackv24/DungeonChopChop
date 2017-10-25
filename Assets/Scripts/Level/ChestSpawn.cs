using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawn : MonoBehaviour
{
	public delegate void NormalEvent();
	public event NormalEvent OnChestSpawned;

	public enum ChestType
	{
		Normal,
		Locked,
		Dungeon
	}

	public ChestType chestType;

	public enum SpawnType
	{
		Probability,
		AlwaysSpawn,
		ClearRoom
	}

	public SpawnType spawnType;

	private bool spawned = false;
	private bool spawnOnClear = false;

	public bool Spawn()
	{
		if(LevelVars.Instance && !spawned)
		{
			GameObject prefab = null;

			switch(chestType)
			{
				case ChestType.Normal:
					prefab = LevelVars.Instance.normalChestPrefab;
					break;
				case ChestType.Locked:
					prefab = LevelVars.Instance.lockedChestPrefab;
					break;
				case ChestType.Dungeon:
					prefab = LevelVars.Instance.dungeonChestPrefab;
					break;
			}

			if (prefab)
			{
				GameObject obj = Instantiate(prefab, transform);
                obj.transform.localPosition = Vector3.zero;

                spawned = true;

				if(spawnOnClear)
				{
					MapTracker tracker = obj.GetComponent<MapTracker>();

					if (tracker)
						tracker.Register();
				}

				PersistentObject persistent = GetComponent<PersistentObject>();
                if (persistent)
                {
                    persistent.Setup();
                }
			}
			else
				Debug.LogWarning("Chest could not spawn, no prefab assigned in LevelVars!");
		}

		if(spawned)
		{
			if (OnChestSpawned != null)
				OnChestSpawned();
		}

		return spawned;
	}

	public void SetSpawnOnClear(LevelTile parentTile)
	{
		EnemySpawner spawner = parentTile.GetComponentInChildren<EnemySpawner>();

		if(spawner)
		{
			spawner.OnEnemiesDefeated += delegate { Spawn(); };
		}

		spawnOnClear = true;
	}
}
