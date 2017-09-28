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

        if (enemyAttack.attackingType != TypesOfAttack.Nothing)
        {
            if (enemyAttack.attackingType == TypesOfAttack.BasicShootIntervals)
            {
                enemyAttack.timeTillInterval = EditorGUILayout.FloatField("Time till Interval in Secs: ", enemyAttack.timeTillInterval); 
            }
            else if (enemyAttack.attackingType == TypesOfAttack.ShootCircleIntervals)
            {
                enemyAttack.timeTillInterval = EditorGUILayout.FloatField("Time till Interval in Secs: ", enemyAttack.timeTillInterval); 
                enemyAttack.projAmount = EditorGUILayout.IntField("Amount to shoot in circle: ", enemyAttack.projAmount); 
            }
            else if (enemyAttack.attackingType == TypesOfAttack.BasicShootRandIntervals)
            {
                enemyAttack.minInterval = EditorGUILayout.FloatField("Min Interval in Secs: ", enemyAttack.minInterval); 
                enemyAttack.maxInterval = EditorGUILayout.FloatField("Max Interval in Secs: ", enemyAttack.maxInterval);
            }
            else if (enemyAttack.attackingType == TypesOfAttack.ShootCircleRandIntervals)
            {
                enemyAttack.projAmount = EditorGUILayout.IntField("Amount to shoot in circle: ", enemyAttack.projAmount); 
                enemyAttack.minInterval = EditorGUILayout.FloatField("Min Interval in Secs: ", enemyAttack.minInterval); 
                enemyAttack.maxInterval = EditorGUILayout.FloatField("Max Interval in Secs: ", enemyAttack.maxInterval);
            }
            enemyAttack.projecticle = (GameObject)EditorGUILayout.ObjectField("Projectile", enemyAttack.projecticle, typeof(GameObject), true);
            enemyAttack.shootPosition = (GameObject)EditorGUILayout.ObjectField("Shoot Position", enemyAttack.shootPosition, typeof(GameObject), true);
            enemyAttack.thrust = EditorGUILayout.FloatField("Thrust of projectile", enemyAttack.thrust);
        }
	}
}