using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReplaceMaterialWindow : EditorWindow
{
	private Material currentMaterial;
	private Material replaceMaterial;

	private bool shadows = true;

	[MenuItem("Jack's Helper Functions/Replace Material")]
	static void Init()
	{
		ReplaceMaterialWindow window = (ReplaceMaterialWindow)EditorWindow.GetWindow(typeof(ReplaceMaterialWindow));
		window.Show();
	}

	void OnGUI()
	{
		GUILayout.Label("Replace Material", EditorStyles.boldLabel);

		currentMaterial = (Material)EditorGUILayout.ObjectField("Material TO replace", currentMaterial, typeof(Material), false);
		replaceMaterial = (Material)EditorGUILayout.ObjectField("Material to replace WITH", replaceMaterial, typeof(Material), false);

		EditorGUILayout.Space();

		shadows = EditorGUILayout.Toggle("Enable Shadows", shadows);

		EditorGUILayout.Space();

		if (GUILayout.Button("Replace") && EditorUtility.DisplayDialog("Confirm", "Are you SURE you want to do this? Please make sure to apply all prefabs aferwards!", "I'm sure", "No, I'm scared"))
		{
			Replace();
		}
	}

	void Replace()
	{
		GameObject[] parents = Selection.gameObjects;

		List<Renderer> renderers = new List<Renderer>();

		foreach(GameObject parent in parents)
		{
			Renderer[] rends= parent.GetComponentsInChildren<Renderer>();

			foreach(Renderer rend in rends)
			{
				if (rend.sharedMaterial == currentMaterial)
					renderers.Add(rend);
			}
		}

		foreach(Renderer rend in renderers)
		{
			rend.material = replaceMaterial;

			rend.receiveShadows = shadows;
		}
	}
}

