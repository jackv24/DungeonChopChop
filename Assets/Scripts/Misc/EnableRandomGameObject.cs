using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomGameObject : MonoBehaviour
{
	public GameObject[] gameObjects;

	public bool useLevelGenerator = true;

	void Start()
	{
		foreach (GameObject obj in gameObjects)
			obj.SetActive(false);

		int index;
		if (useLevelGenerator)
			index = LevelGenerator.Random.Next(0, gameObjects.Length);
		else
			index = Random.Range(0, gameObjects.Length);

		gameObjects[index].SetActive(true);
	}
}
