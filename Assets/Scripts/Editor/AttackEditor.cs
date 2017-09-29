using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(EnemyAttack), true)]
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
				//enemyAttack.timeTillInterval = EditorGUILayout.FloatField("Time till Interval in Secs: ", enemyAttack.timeTillInterval); 
				EditorGUILayout.PropertyField(serializedObject.FindProperty("timeTillInterval"), new GUIContent("Time till Interval in Secs: "));
            }
            else if (enemyAttack.attackingType == TypesOfAttack.ShootCircleIntervals)
            {
				//enemyAttack.timeTillInterval = EditorGUILayout.FloatField("Time till Interval in Secs: ", enemyAttack.timeTillInterval); 
				//enemyAttack.projAmount = EditorGUILayout.IntField("Amount to shoot in circle: ", enemyAttack.projAmount);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("timeTillInterval"), new GUIContent("Time till Interval in Secs"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("projAmount"), new GUIContent("Amount to shoot in circle"));
			}
            else if (enemyAttack.attackingType == TypesOfAttack.BasicShootRandIntervals)
            {
				//enemyAttack.minInterval = EditorGUILayout.FloatField("Min Interval in Secs: ", enemyAttack.minInterval); 
				//enemyAttack.maxInterval = EditorGUILayout.FloatField("Max Interval in Secs: ", enemyAttack.maxInterval);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("minInterval"), new GUIContent("Min Interval in Secs"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("maxInterval"), new GUIContent("Max Interval in Secs"));
			}
            else if (enemyAttack.attackingType == TypesOfAttack.ShootCircleRandIntervals)
            {
				//enemyAttack.projAmount = EditorGUILayout.IntField("Amount to shoot in circle: ", enemyAttack.projAmount); 
				//enemyAttack.minInterval = EditorGUILayout.FloatField("Min Interval in Secs: ", enemyAttack.minInterval); 
				//enemyAttack.maxInterval = EditorGUILayout.FloatField("Max Interval in Secs: ", enemyAttack.maxInterval);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("projAmount"), new GUIContent("Amount to shoot in circle"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("minInterval"), new GUIContent("Min Interval in Secs"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("maxInterval"), new GUIContent("Max Interval in Secs"));
			}
			//enemyAttack.projecticle = (GameObject)EditorGUILayout.ObjectField("Projectile", enemyAttack.projecticle, typeof(GameObject), true);
			//enemyAttack.shootPosition = (GameObject)EditorGUILayout.ObjectField("Shoot Position", enemyAttack.shootPosition, typeof(GameObject), true);
			//enemyAttack.thrust = EditorGUILayout.FloatField("Thrust of projectile", enemyAttack.thrust);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("projecticle"), new GUIContent("Projectile"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("shootPosition"), new GUIContent("Shoot Position"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("thrust"), new GUIContent("Thrust of projectile"));
		}

		serializedObject.ApplyModifiedProperties();
	}
}