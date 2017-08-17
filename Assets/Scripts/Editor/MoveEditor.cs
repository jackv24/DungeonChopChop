using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(EnemyNavMove))]
public class MoveEditor : Editor {

    private EnemyNavMove enemyMove;

	void OnEnable()
	{
        enemyMove = (EnemyNavMove)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
        if (enemyMove.movingType == TypesOfMoving.Roam) {
            enemyMove.timeBetweenRoam = EditorGUILayout.FloatField ("Time Between Roam Change: ", enemyMove.timeBetweenRoam); 
		}
	}
}
	
