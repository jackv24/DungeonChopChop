using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(TileQuest))]
public class TileQuestEditor : Editor {

    private TileQuest tileQuest;

    void OnEnable()
    {
        tileQuest = (TileQuest)target;
    }
	
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (tileQuest.challengeType == ChallengeType.KillEnemiesInTime)
        {
            tileQuest.timeToDoTask = EditorGUILayout.FloatField("Time to do task", tileQuest.timeToDoTask);
        }

        else if (tileQuest.challengeType == ChallengeType.LeversInTime)
        {
            
            tileQuest.timeToDoTask = EditorGUILayout.FloatField("Time to do task", tileQuest.timeToDoTask);

        }
    }
}
