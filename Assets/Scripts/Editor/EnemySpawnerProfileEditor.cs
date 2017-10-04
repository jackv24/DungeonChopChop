using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(EnemySpawner), true)]
public class EnemySpawnerProfileEditor : Editor
{
	private void OnSceneGUI()
	{
		EnemySpawner spawner = (EnemySpawner)target;

		if (spawner.currentProfileIndex < spawner.profiles.Length && spawner.currentProfileIndex >= 0)
		{
			EnemySpawner.Profile profile = spawner.profiles[spawner.currentProfileIndex];

			foreach (EnemySpawner.Profile.Spawn spawn in profile.spawns)
			{
				EditorGUI.BeginChangeCheck();
				Vector3 position = Handles.PositionHandle(spawner.transform.TransformPoint(spawn.position), Quaternion.identity);

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(spawner, "Changed spawn position");

					spawn.position = spawner.transform.InverseTransformPoint(position);
				}

				Handles.Label(position, new GUIContent(spawn.enemyPrefab ? spawn.enemyPrefab.name : "No prefab assigned"), EditorStyles.whiteBoldLabel);
			}
		}
	}
}