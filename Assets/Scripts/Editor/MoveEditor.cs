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
			enemyMove.power = EditorGUILayout.FloatField ("The amount of power on impulse: ", enemyMove.power); 
			enemyMove.chargeUptime = EditorGUILayout.FloatField ("The time it takes to charge up: ", enemyMove.chargeUptime); 
		}
		if (enemyMove.moveTimes == MoveTimes.Hop) {
			enemyMove.power = EditorGUILayout.FloatField ("The amount of power on impulse: ", enemyMove.power); 
			enemyMove.jumpPower = EditorGUILayout.FloatField ("The amount of jump power: ", enemyMove.jumpPower); 
		}
		if (enemyMove.movementWhilstIdle == MovementWhilstIdle.Roam) {
			enemyMove.minAdjTime = EditorGUILayout.FloatField ("Min time between direction change: ", enemyMove.minAdjTime); 
			enemyMove.maxAdjTime = EditorGUILayout.FloatField ("Max time between direction change: ", enemyMove.maxAdjTime); 
		}

		if (enemyMove.movingType == TypesOfMoving.Follow) {
			if (enemyMove.moveTimes == MoveTimes.Charge) {
				if (enemyMove.moveDistances == MoveDistances.InSight) {
					EditorGUILayout.HelpBox ("Warning: This movement doesn't exist", MessageType.Warning);
				}
			}
		}
		if (enemyMove.movingType == TypesOfMoving.Follow) {
			if (enemyMove.moveTimes == MoveTimes.Stutter) {
				if (enemyMove.moveDistances == MoveDistances.InSight || enemyMove.moveDistances == MoveDistances.Radius) {
					EditorGUILayout.HelpBox ("Warning: This movement doesn't exist", MessageType.Warning);
				}
			}
			if (enemyMove.moveTimes == MoveTimes.Hop) {
				if (enemyMove.moveDistances == MoveDistances.InSight || enemyMove.moveDistances == MoveDistances.Radius) {
					EditorGUILayout.HelpBox ("Warning: This movement doesn't exist", MessageType.Warning);
				}
			}
		}
		if (enemyMove.movingType == TypesOfMoving.Roam) {
			if (enemyMove.moveTimes == MoveTimes.Stutter || enemyMove.moveTimes == MoveTimes.Charge) {
				EditorGUILayout.HelpBox ("Warning: This movement doesn't exist", MessageType.Warning);
			} else {
				if (enemyMove.moveDistances == MoveDistances.InSight) {
					EditorGUILayout.HelpBox ("Warning: This movement doesn't exist", MessageType.Warning);
				}
			}
		}
		if (enemyMove.movingType == TypesOfMoving.Static) {
			if (enemyMove.moveTimes != MoveTimes.Charge && enemyMove.moveTimes != MoveTimes.Constant) {
				EditorGUILayout.HelpBox ("Warning: This movement doesn't exist", MessageType.Warning);
			} else {
				if (enemyMove.moveDistances == MoveDistances.InSight || enemyMove.moveDistances == MoveDistances.Radius) {
					EditorGUILayout.HelpBox ("Warning: This movement doesn't exist", MessageType.Warning);
				}
			}
		}
	}
}
	
