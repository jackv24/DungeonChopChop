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
	private Dictionary<string, bool> globalBools = new Dictionary<string, bool>();

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
		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));

		currentSceneIndex = index;
	}

	public void SetGlobalMultiplier(string key, float value)
	{
		globalMultipliers[key] = value;
	}

	public void SetGlobalBool(string key, bool value)
	{
		globalBools[key] = value;
	}

	public float GetGlobalMultiplier(string key)
	{
		if (globalMultipliers.ContainsKey(key))
			return globalMultipliers[key];
		else
			return 1.0f;
	}

	public bool GetGlobalBool(string key)
	{
		if (globalBools.ContainsKey (key))
			return globalBools [key];
		else
			return false;
	}
}
