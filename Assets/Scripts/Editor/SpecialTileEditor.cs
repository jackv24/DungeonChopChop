using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpecialTile))]
public class SpecialTileEditor : Editor
{
    SpecialTile specialTile;

    int length;

    private void OnEnable()
	{
        specialTile = (SpecialTile)target;

		length = System.Enum.GetNames(typeof(SpecialTile.SpecialType)).Length;

		//if length has changed, copy old values to new array
		if(length != specialTile.replaceTiles.Length)
		{
			//Get reference to old tile array and make new array at the correct size
            SpecialTile.ReplaceBiome[] oldTiles = specialTile.replaceTiles;
            SpecialTile.ReplaceBiome[] newTiles = new SpecialTile.ReplaceBiome[length];

            Debug.Log("Special Tile array has changed length from " + oldTiles.Length + " to " + length);

            //Copy over values to new array
            for (int i = 0; i < Mathf.Min(oldTiles.Length, newTiles.Length); i++)
                newTiles[i] = oldTiles[i];

			//Assign new array back to original
            specialTile.replaceTiles = newTiles;
        }
    }

	public override void OnInspectorGUI()
	{
        EditorGUILayout.LabelField("Special Tile Prefabs", EditorStyles.largeLabel);

        SerializedProperty property = serializedObject.FindProperty("replaceTiles");

        for (int i = 0; i < specialTile.replaceTiles.Length; i++)
		{
            EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), new GUIContent(System.Enum.GetName(typeof(SpecialTile.SpecialType), i)), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
