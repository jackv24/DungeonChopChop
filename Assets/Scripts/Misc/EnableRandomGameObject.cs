using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomGameObject : MonoBehaviour
{
	public GameObject[] gameObjects;

	void Start()
	{
		foreach (GameObject obj in gameObjects)
			obj.SetActive(false);

		gameObjects[Random.Range(0, gameObjects.Length)].SetActive(true);
	}
}
