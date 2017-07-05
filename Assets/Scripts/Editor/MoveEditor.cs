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
		if (enemyMove.movingType == TypesOfMoving.Roam) {
			enemyMove.minAdjTime = EditorGUILayout.FloatField ("Min time between direction change: ", enemyMove.minAdjTime); 
			enemyMove.maxAdjTime = EditorGUILayout.FloatField ("Max time between direction change: ", enemyMove.maxAdjTime); 
		}
		if (enemyMove.moveTimes == MoveTimes.Stutter) {
			enemyMove.power = EditorGUILayout.FloatField ("The amount of power on impulse: ", enemyMove.power); 
			enemyMove.timeBetweenStutter = EditorGUILayout.FloatField ("Time between each stutter: ", enemyMove.timeBetweenStutter); 
		}
		if (enemyMove.moveTimes == MoveTimes.Charge) {

		}
	}
}
	
