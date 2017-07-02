using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(EnemyAttack))]
public class AttackEditor : Editor {

	private EnemyAttack enemyAttack;

	void OnEnable()
	{
		enemyAttack = (EnemyAttack)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.BeginHorizontal();
		if (enemyAttack.attackingType == TypesOfAttack.BasicShootIntervals) 
		{
			enemyAttack.timeTillInterval = EditorGUILayout.FloatField ("Time till Interval in Secs: ", enemyAttack.timeTillInterval); 
		} 
		else if (enemyAttack.attackingType == TypesOfAttack.ShootCircleIntervals) 
		{
			enemyAttack.timeTillInterval = EditorGUILayout.FloatField ("Time till Interval in Secs: ", enemyAttack.timeTillInterval); 
			enemyAttack.projAmount = EditorGUILayout.IntField ("Amount to shoot in circle: ", enemyAttack.projAmount); 
		}
		else if (enemyAttack.attackingType == TypesOfAttack.BasicShootRandom) 
		{
		}
		else if (enemyAttack.attackingType == TypesOfAttack.ShootCircleRandom) 
		{
			enemyAttack.projAmount = EditorGUILayout.IntField ("Amount to shoot in circle: ", enemyAttack.projAmount); 
		}
	}
}