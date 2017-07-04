using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    private LevelGenerator generator;

    void OnEnable()
    {
        generator = (LevelGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Please note that preview performance will be significantly worse than in-game performance, due to the large number of tiles on-screen and the lack of static batching", MessageType.Warning);

		EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate Preview"))
        {
            generator.Generate();
            EditorSceneManager.MarkAllScenesDirty();
        }
        if (GUILayout.Button("Clear"))
        {
            generator.Clear();
            EditorSceneManager.MarkAllScenesDirty();
        }
        EditorGUILayout.EndHorizontal();
    }
}
