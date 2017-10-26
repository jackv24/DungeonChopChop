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

		if (GUILayout.Button("Replace") && EditorUtility.DisplayDialog("Confirm", "Are you SURE you want to do this? Please make sure to apply all prefabs aferwards!", "I'm sure", "No, I'm scared"))
		{
			Replace();
		}
	}

	void Replace()
	{
		GameObject[] parents = Selection.gameObjects;

		List<AudioSource> sources = new List<AudioSource>();

		foreach(GameObject parent in parents)
		{
			AudioSource[] srcs = parent.GetComponentsInChildren<AudioSource>();

			foreach(AudioSource source in srcs)
			{
				if (source.outputAudioMixerGroup == currentMixer)
					sources.Add(source);
			}
		}

		foreach(AudioSource source in sources)
		{
            source.outputAudioMixerGroup = replaceMixer;
        }
	}
}

