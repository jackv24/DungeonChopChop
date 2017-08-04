using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public int startSceneIndex = 1;

	private int currentSceneIndex = -1;

	private Dictionary<string, float> globalMultipliers = new Dictionary<string, float>();

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		if(SceneManager.sceneCount == 1)
			StartCoroutine (SetupGame (startSceneIndex));
	}

	IEnumerator SetupGame(int index)
	{
		yield return SceneManager.LoadSceneAsync (index, LoadSceneMode.Additive);
		currentSceneIndex = index;
		// After first scene is loaded, do game setup
	}

	public void ChangeScene(int newSceneIndex)
	{
		StartCoroutine (ChangeSceneSequence (newSceneIndex));
	}

	IEnumerator ChangeSceneSequence(int index)
	{
		yield return SceneManager.UnloadSceneAsync (currentSceneIndex);

		//stuff inbetween scenes

		yield return SceneManager.LoadSceneAsync (index, LoadSceneMode.Additive);

		currentSceneIndex = index;
	}

	public void SetGlobalMultiplier(string key, float value)
	{
		globalMultipliers[key] = value;
	}

	public float GetGlobalMultiplier(string key)
	{
		if (globalMultipliers.ContainsKey(key))
			return globalMultipliers[key];
		else
			return 1.0f;
	}
}
