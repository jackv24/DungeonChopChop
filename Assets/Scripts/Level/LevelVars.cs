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
	public GameObject droppedCharmPrefab;

	private void Awake()
	{
		Instance = this;
	}
}
