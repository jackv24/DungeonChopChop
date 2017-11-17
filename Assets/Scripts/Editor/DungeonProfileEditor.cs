using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(DungeonGeneratorProfile), true)]
public class DungeonProfileEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		SerializedProperty biome = serializedObject.FindProperty("dungeonBiome");

		if(biome.enumValueIndex == (int)LevelTile.Biomes.BossDungeon)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("bossTile"));
		}

		serializedObject.ApplyModifiedProperties();
	}
}
