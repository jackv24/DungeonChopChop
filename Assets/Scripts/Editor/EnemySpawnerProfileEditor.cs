using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(EnemySpawner), true)]
public class EnemySpawnerProfileEditor : Editor
{
	private int currentIndex = 0;

	private EnemySpawner spawner;

	private void OnEnable()
	{
		spawner = (EnemySpawner)target;

		currentIndex = spawner.currentProfileIndex;
	}

	public override void OnInspectorGUI()
	{
		if (!Application.isPlaying)
		{
			EditorGUILayout.LabelField(string.Format("Current Profile: {0}/{1}", spawner.profiles.Count > 0 ? currentIndex + 1 : 0, spawner.profiles.Count));

			EditorGUILayout.BeginHorizontal();
			Color defaultColor = GUI.backgroundColor;
			if (GUILayout.Button("Previous"))
			{
				currentIndex--;

				if (currentIndex < 0)
					currentIndex = spawner.profiles.Count - 1;
			}
			if (GUILayout.Button("Next"))
			{
				currentIndex++;

				if (currentIndex >= spawner.profiles.Count)
					currentIndex = 0;
			}
			GUI.backgroundColor = new Color(0.5f, 1, 0.5f);
			if (GUILayout.Button("Add New"))
			{
				//Get previous profile and add again to end (duplicates data since this is a struct)
				EnemySpawner.Profile previous = spawner.profiles[spawner.profiles.Count - 1];
				spawner.profiles.Add(previous);

				currentIndex = spawner.profiles.Count - 1;
			}
			GUI.backgroundColor = new Color(1, 0.5f, 0.5f);
			if (GUILayout.Button("Remove Current") && EditorUtility.DisplayDialog("Confirm Profile Deletion", "Are you sure you want to delete this profile?\nThis can not be undone!", "Yes, DELETE", "Cancel"))
			{
				spawner.profiles.RemoveAt(currentIndex);

				if (currentIndex >= spawner.profiles.Count)
					currentIndex = spawner.profiles.Count - 1;
			}
			GUI.backgroundColor = defaultColor;
			EditorGUILayout.EndHorizontal();

			//Check if anything has changed and not applied to the prefab
			PropertyModification[] propertyModifications = PrefabUtility.GetPropertyModifications(spawner.gameObject);
			int modified = 0;
			foreach (var modification in propertyModifications)
			{
				if (modification.target.name == spawner.gameObject.name &&
						  (modification.propertyPath.Contains("m_LocalPosition") || modification.propertyPath.Contains("m_LocalRotation") || modification.propertyPath == "m_RootOrder"))
				{
					// Do not consider this as a modification, since it's a modification on the root's transform.
				}
				else
					modified++;
			}

			//Show warnign if there are instance changes
			if (modified > 0)
				EditorGUILayout.HelpBox(string.Format("There are {0} unapplied prefab changes on this instance!", modified), MessageType.Warning);

			EditorGUILayout.Space();
			SerializedProperty profiles = serializedObject.FindProperty("profiles");
			if (profiles.arraySize > 0 && currentIndex < profiles.arraySize)
			{
				SerializedProperty profile = profiles.GetArrayElementAtIndex(currentIndex);
				if (profile != null)
				{
					SerializedProperty spawns = profile.FindPropertyRelative("spawns");

					EditorGUILayout.PropertyField(spawns, new GUIContent("Profile " + (currentIndex + 1) + " Spawn Points"), true);
				}
			}
		}
		else
			EditorGUILayout.HelpBox("Can not edit spawner in play mode!", MessageType.Warning);

		serializedObject.ApplyModifiedProperties();
	}

	private void OnSceneGUI()
	{
		if (currentIndex < spawner.profiles.Count && currentIndex >= 0)
		{
			EnemySpawner.Profile profile = spawner.profiles[currentIndex];

			if (profile.spawns != null)
			{
				foreach (EnemySpawner.Profile.Spawn spawn in profile.spawns)
				{
					EditorGUI.BeginChangeCheck();
					Vector3 position = Handles.PositionHandle(spawner.transform.TransformPoint(spawn.position), spawner.transform.rotation);

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
}