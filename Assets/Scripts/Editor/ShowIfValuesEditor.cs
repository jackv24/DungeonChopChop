using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(EnemyMove))]
public class ShowIfValuesEditor : Editor {

	private EnemyMove enemyMove;

	void OnEnable()
	{
		enemyMove = (EnemyMove)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		//EditorGUILayout.BeginHorizontal();
		if (enemyMove.movingType == TypesOfMoving.BasicFollow) 
		{
		} 
		else if (enemyMove.movingType == TypesOfMoving.FollowInRadius) 
		{
			enemyMove.radius = EditorGUILayout.FloatField ("Detect Radius: ", enemyMove.radius); 
		}
		else if (enemyMove.movingType == TypesOfMoving.FollowWithIntervals) 
		{
			enemyMove.timeTillInterval = EditorGUILayout.FloatField ("Time till Interval in Secs: ", enemyMove.timeTillInterval); 
			enemyMove.interval = EditorGUILayout.FloatField ("Interval wait in Secs: ", enemyMove.interval); 
		}
	}
}
	
