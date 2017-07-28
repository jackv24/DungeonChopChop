using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public Transform[] spawnPoints;

	public Helper.Probability[] enemies;

	public void Spawn()
	{
		foreach (Transform spawn in spawnPoints)
		{
			GameObject enemyPrefab = Helper.GetRandomByProbability(enemies);

			if (enemyPrefab)
			{
				GameObject enemy = ObjectPooler.GetPooledObject(enemyPrefab);
				enemy.transform.position = spawn.position;
			}
		}
	}
}
