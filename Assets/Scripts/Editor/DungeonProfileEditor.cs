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
			EditorGUILayout.HelpBox("Please make sure that the Start Tile is the Boss tile!", MessageType.Info);
		}
		else if(biome.enumValueIndex == (int)LevelTile.Biomes.Dungeon1 ||
				biome.enumValueIndex == (int)LevelTile.Biomes.Dungeon2 ||
				biome.enumValueIndex == (int)LevelTile.Biomes.Dungeon3 ||
				biome.enumValueIndex == (int)LevelTile.Biomes.Dungeon4)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("keyTilePrefab"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("chestTilePrefab"));
		}
		else
		{
			EditorGUILayout.HelpBox("Biome selected is not a dungeon biome! Please change to a dungeon biome, or it will be automatically changed to Dungeon1 at runtime", MessageType.Error);
		}

		serializedObject.ApplyModifiedProperties();
	}
}
