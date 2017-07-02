using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(EnemyDeath))]
public class DeathEditor : Editor {

	private EnemyDeath enemyDeath;

	void OnEnable()
	{
		enemyDeath = (EnemyDeath)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (enemyDeath.deathType == TypesOfDeath.SplitsIntoAnotherEnemy) 
		{
			enemyDeath.amountToSplit = EditorGUILayout.IntField ("Amount to split", enemyDeath.amountToSplit);
			enemyDeath.splitEnemy = (GameObject)EditorGUILayout.ObjectField ("Split Enemy", enemyDeath.splitEnemy, typeof(GameObject), true);
		}
	}
}