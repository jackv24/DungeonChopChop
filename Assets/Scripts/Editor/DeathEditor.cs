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
            enemyDeath.amountToSplit = EditorGUILayout.IntField("Amount to split", enemyDeath.amountToSplit);
            enemyDeath.splitEnemy = (GameObject)EditorGUILayout.ObjectField("Split Enemy", enemyDeath.splitEnemy, typeof(GameObject), true);
        }
        else if (enemyDeath.deathType == TypesOfDeath.StatusExplode)
        {
            enemyDeath.explodeRadius = EditorGUILayout.FloatField("Explode Radius", enemyDeath.explodeRadius);
            enemyDeath.statusType = (StatusType)EditorGUILayout.EnumPopup("Status Type", enemyDeath.statusType);
            enemyDeath.damagePerTick = EditorGUILayout.FloatField("Damage per tick", enemyDeath.damagePerTick);
            enemyDeath.timeBetweenTick = EditorGUILayout.FloatField("Time between tick", enemyDeath.timeBetweenTick);
            enemyDeath.duration = EditorGUILayout.FloatField("Duration of status", enemyDeath.duration);
        }
        else if (enemyDeath.deathType == TypesOfDeath.DamageExplode)
        {
            enemyDeath.explodeRadius = EditorGUILayout.FloatField("Explode Radius", enemyDeath.explodeRadius);
            enemyDeath.damageOnExplode = EditorGUILayout.FloatField("Damage on Explode", enemyDeath.damageOnExplode);
        }
	}
}