using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public Transform[] spawnPoints;

	public Helper.Probability[] enemies;

	private List<GameObject> spawnedEnemies = new List<GameObject>();

	[Space()]
	[Tooltip("How many tiles must be entered until this one respawns enemies?")]
	public int respawnTileCount = 5;
	private int tilesLeftUntilRespawn = 0;

	public void Spawn()
	{
		if (tilesLeftUntilRespawn > 0)
			return;

		foreach (Transform spawn in spawnPoints)
		{
			GameObject enemyPrefab = Helper.GetRandomByProbability(enemies);

			if (enemyPrefab)
			{
				GameObject enemy = ObjectPooler.GetPooledObject(enemyPrefab);
				enemy.transform.position = spawn.position + Vector3.up;
				enemy.transform.rotation = Quaternion.identity;

				spawnedEnemies.Add(enemy);
			}
		}
	}

	public void Despawn()
	{
		bool tileClear = true;

		foreach (GameObject enemy in spawnedEnemies)
		{
			if (enemy.activeSelf)
			{
				tileClear = false;

				//TODO: Save which enemies are still alive for respawn later
				enemy.SetActive(false);
			}
		}

		spawnedEnemies.Clear();

		//If tile was just cleared, setup tile reset counter
		if (tileClear && tilesLeftUntilRespawn <= 0)
		{
			tilesLeftUntilRespawn = respawnTileCount;

			LevelGenerator.Instance.OnTileEnter += MinusTilesLeft;
		}
	}

	void MinusTilesLeft()
	{
		tilesLeftUntilRespawn--;

		//If no tiles are left, remove reset counter
		if (tilesLeftUntilRespawn <= 0)
			LevelGenerator.Instance.OnTileEnter -= MinusTilesLeft;
	}
}
