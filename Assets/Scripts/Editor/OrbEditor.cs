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
            orb.healthAmount = EditorGUILayout.FloatField("Health Amount", orb.healthAmount);
        }
        else if (orb.type == OrbType.Cure)
        {
            orb.cureAmount = EditorGUILayout.IntField("Cure Amount", orb.cureAmount);
        }
    }
}
