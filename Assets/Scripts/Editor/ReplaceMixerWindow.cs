using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;

public class ReplaceMixerWindow : EditorWindow
{
	private AudioMixerGroup currentMixer;
	private AudioMixerGroup replaceMixer;

	private bool shadows = true;

	[MenuItem("Jack's Helper Functions/Replace Mixers")]
	static void Init()
	{
		ReplaceMixerWindow window = (ReplaceMixerWindow)EditorWindow.GetWindow(typeof(ReplaceMixerWindow));
		window.Show();
	}

	void OnGUI()
	{
		GUILayout.Label("Replace Mixers", EditorStyles.boldLabel);

		currentMixer = (AudioMixerGroup)EditorGUILayout.ObjectField("Mixer TO replace", currentMixer, typeof(AudioMixerGroup), false);
		replaceMixer = (AudioMixerGroup)EditorGUILayout.ObjectField("Mixer to replace WITH", replaceMixer, typeof(AudioMixerGroup), false);

		EditorGUILayout.Space();

		if (GUILayout.Button("Replace Selected") && EditorUtility.DisplayDialog("Confirm", "Are you SURE you want to do this? Please make sure to apply all prefabs aferwards!", "I'm sure", "No, I'm scared"))
		{
			Replace(true);
		}

		if(GUILayout.Button("Replace ALL (in project)") && EditorUtility.DisplayDialog("Confirm", "Are you SURE you want to do this? It will change a lot of files!", "I'm sure", "No, I'm scared"))
		{
            Replace(false);
        }
	}

	void Replace(bool selected)
	{
		List<AudioSource> sources = new List<AudioSource>();

        if (selected)
        {
			GameObject[] parents = Selection.gameObjects;

            foreach (GameObject parent in parents)
            {
                AudioSource[] srcs = parent.GetComponentsInChildren<AudioSource>();

                sources.AddRange(srcs);
            }
        }
		else
		{
            AudioSource[] srcs = Resources.FindObjectsOfTypeAll<AudioSource>();

            sources.AddRange(srcs);
        }

        foreach(AudioSource source in sources)
		{
            if (source.outputAudioMixerGroup == currentMixer)
            {
                source.outputAudioMixerGroup = replaceMixer;
                EditorUtility.SetDirty(source);
            }
        }
	}
}

