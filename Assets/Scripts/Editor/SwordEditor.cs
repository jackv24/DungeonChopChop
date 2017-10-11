using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(SwordStats))]
public class SwordEditor : Editor {

    private SwordStats swordStats;

    void OnEnable()
    {
        swordStats = (SwordStats)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (swordStats.weaponEffect == WeaponEffect.Burn || swordStats.weaponEffect == WeaponEffect.Poison || swordStats.weaponEffect == WeaponEffect.SlowDeath)
        {
            swordStats.damagePerTick = EditorGUILayout.FloatField("Damage per tick", swordStats.damagePerTick);
            swordStats.duration = EditorGUILayout.FloatField("Duration", swordStats.duration);
            swordStats.timeBetweenEffect = EditorGUILayout.FloatField("Time between tick", swordStats.timeBetweenEffect);
        }
        else if (swordStats.weaponEffect == WeaponEffect.Ice)
        {
            swordStats.duration = EditorGUILayout.FloatField("Duration", swordStats.duration);;
        }
        else if (swordStats.weaponEffect == WeaponEffect.Sandy)
        {
            swordStats.duration = EditorGUILayout.FloatField("Duration", swordStats.duration);
            swordStats.speedDamper = EditorGUILayout.FloatField("Speed Damping", swordStats.speedDamper);
        }
    }
}
