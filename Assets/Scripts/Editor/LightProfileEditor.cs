using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightProfile))]
public class LightProfileEditor : Editor
{
	public override void OnInspectorGUI()
	{
        base.OnInspectorGUI();

        EditorGUILayout.Space();
		if(GUILayout.Button("Preview", GUILayout.Height(30)))
		{
            BiomeLighting lighting = FindObjectOfType<BiomeLighting>();

			if(lighting)
			{
                LightProfile profile = (LightProfile)target;

                lighting.UpdateLighting(profile);
            }
			else
			{
                Debug.LogError("Could not find a BiomeLighting instance!");
            }
        }
    }
}
