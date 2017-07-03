using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(EnemyMove))]
public class MoveEditor : Editor {

	private EnemyMove enemyMove;

	void OnEnable()
	{
		enemyMove = (EnemyMove)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (enemyMove.moveDistances == MoveDistances.Radius) {
			enemyMove.radius = EditorGUILayout.FloatField ("Detect Radius: ", enemyMove.radius); 
		}
		if (enemyMove.moveTimes == MoveTimes.Interval) {
			enemyMove.timeTillInterval = EditorGUILayout.FloatField ("Time till Interval in Secs: ", enemyMove.timeTillInterval); 
			enemyMove.interval = EditorGUILayout.FloatField ("Interval wait in Secs: ", enemyMove.interval); 
		}
		if (enemyMove.moveDistances == MoveDistances.InSight) {
			enemyMove.distance = EditorGUILayout.FloatField ("Distance can see player: ", enemyMove.distance); 
		}
	}
}
	
