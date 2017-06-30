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

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate"))
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
