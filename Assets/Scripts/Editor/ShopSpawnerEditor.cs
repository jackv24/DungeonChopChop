using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShopSpawner))]
public class ShopSpawnerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Generate"))
		{
			//Preview generation in-editor. Will be regenerated in-game
			((ShopSpawner)target).Generate();
		}
		if(GUILayout.Button("Clear"))
		{
			((ShopSpawner)target).DeleteChildren();
		}
		EditorGUILayout.EndHorizontal();
	}
}
