using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawn : MonoBehaviour
{
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
				GameObject obj = Instantiate(prefab, transform.parent);
				obj.transform.localPosition = transform.localPosition;

				spawned = true;
			}
			else
				Debug.LogWarning("Chest could not spawn, no prefab assigned in LevelVars!");
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
	}
}
