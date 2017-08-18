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

	}
}
	
