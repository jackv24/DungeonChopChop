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

	private class UndefeatedEnemy
	{
		public UndefeatedEnemy(GameObject prefab, Transform spawnPoint)
		{
			this.prefab = prefab;
			this.spawnPoint = spawnPoint;
		}

		public GameObject prefab;
		public Transform spawnPoint;
	}
	private List<UndefeatedEnemy> undefeatedEnemies = new List<UndefeatedEnemy>();

	public void Spawn()
	{
		if (tilesLeftUntilRespawn > 0)
			return;

		if (undefeatedEnemies.Count <= 0)
		{
			foreach (Transform spawn in spawnPoints)
			{
				GameObject enemyPrefab = Helper.GetRandomByProbability(enemies);

				if (enemyPrefab)
				{
					GameObject enemy = ObjectPooler.GetPooledObject(enemyPrefab);
					enemy.transform.position = spawn.position + Vector3.up;
					enemy.transform.rotation = Quaternion.identity;

					spawnedEnemies.Add(enemy);

					undefeatedEnemies.Add(new UndefeatedEnemy(enemyPrefab, spawn));
				}
			}
		}
		else
		{
			foreach(UndefeatedEnemy undefeatedEnemy in undefeatedEnemies)
			{
				GameObject enemy = ObjectPooler.GetPooledObject(undefeatedEnemy.prefab);
				enemy.transform.position = undefeatedEnemy.spawnPoint.position + Vector3.up;
				enemy.transform.rotation = Quaternion.identity;

				spawnedEnemies.Add(enemy);
			}
		}
	}

	public void Despawn()
	{
		List<UndefeatedEnemy> toRemove = new List<UndefeatedEnemy>();

		for (int i = 0; i < undefeatedEnemies.Count; i++)
		{
			//Add defeated enemies to a list so that iterator is not edited
			if (!spawnedEnemies[i].activeSelf)
				toRemove.Add(undefeatedEnemies[i]);
		}

		//Remove defeated enemies
		foreach (UndefeatedEnemy e in toRemove)
			undefeatedEnemies.Remove(e);

		toRemove.Clear();

		foreach (GameObject enemy in spawnedEnemies)
			enemy.SetActive(false);
	
		spawnedEnemies.Clear();

		//If tile was just cleared, setup tile reset counter
		if (undefeatedEnemies.Count <= 0 && tilesLeftUntilRespawn <= 0)
		{
			tilesLeftUntilRespawn = respawnTileCount;

			LevelGenerator.Instance.OnTileEnter += MinusTilesLeft;
		}
	}

	void MinusTilesLeft()
	{
		tilesLeftUntilRespawn--;

		//If time to reset tile, just clear undefeated enemies
		undefeatedEnemies.Clear();

		//If no tiles are left, remove reset counter
		if (tilesLeftUntilRespawn <= 0)
			LevelGenerator.Instance.OnTileEnter -= MinusTilesLeft;
	}
}
