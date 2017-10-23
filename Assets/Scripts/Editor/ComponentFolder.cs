using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(Folder))]
public class ComponentFolder : Editor {

    Folder fold;

    void OnEnable()
    {
        fold = (Folder)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (UnityEditorInternal.InternalEditorUtility.GetIsInspectorExpanded(fold))
        {
            ShowComponents();
        }
        else
        {
            HideComponents();
        }
    }

    void ShowComponents()
    {
        for (int i = 0; i < fold.components.Length; i++)
        {
            fold.components[i].hideFlags = HideFlags.None;
            Debug.Log(EditorGUIUtility.ObjectContent(fold.components[i], typeof(Component)).text);
            EditorGUIUtility.ObjectContent(fold.components[i], typeof(Component)).text = "<color=blue>" + EditorGUIUtility.ObjectContent(fold.components[i], typeof(Component)).text + "</color>" + "hello";
        }
    }

    void HideComponents()
    {
        for (int i = 0; i < fold.components.Length; i++)
        {
             fold.components[i].hideFlags = HideFlags.HideInInspector;;
        }
    }
}
