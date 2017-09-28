using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenOnEnemyDefeat : MonoBehaviour
{
	private Animator animator;
	private EnemySpawner spawner;

	private bool subscribed = false;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
	{
		spawner = GetComponentInParent<LevelTile>().GetComponentInChildren<EnemySpawner>();

		if (spawner)
		{
			spawner.OnEnemiesDefeated += Open;
			spawner.OnEnemiesSpawned += Close;

			subscribed = true;
		}
		else
			Open();
	}

	void OnDestroy()
	{
		if(subscribed)
		{
			spawner.OnEnemiesDefeated -= Open;
			spawner.OnEnemiesSpawned -= Close;
		}
	}

	void Open()
	{
		animator.SetBool("opened", true);
	}

	void Close()
	{
		animator.SetBool("opened", false);
	}
}
