using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(Orb))]
public class OrbEditor : Editor {
    
    private Orb orb;

    void OnEnable()
    {
        orb = (Orb)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (orb.type == OrbType.Health)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("healthAmount"), new GUIContent("Health Amount"));
        }
        else if (orb.type == OrbType.Cure)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cureAmount"), new GUIContent("Cure Amount"));
        }
    }
}
