using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public Transform[] spawnPoints;

	public Helper.Probability[] enemies;

	private List<GameObject> spawnedEnemies = new List<GameObject>();

	public void Spawn()
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
			}
		}
	}

	public void Despawn()
	{
		foreach (GameObject enemy in spawnedEnemies)
		{
			enemy.SetActive(false);
		}

		spawnedEnemies.Clear();
	}
}
