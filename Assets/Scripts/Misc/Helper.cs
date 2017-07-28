using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
	[System.Serializable]
	public class Probability
	{
		public GameObject prefab;
		public float probability;
	}

	public static GameObject GetRandomByProbability(Probability[] array)
	{
		List<Probability> possibleGameObjects = new List<Probability>(array);

		Probability possibleGameObject = null;

		///Get random enemy with probability
		//Sort possible enemy list by probability
		possibleGameObjects.Sort((x, y) => x.probability.CompareTo(y.probability));

		//Get sum of all probabilities
		float maxProbability = 0;
		foreach (Probability e in possibleGameObjects)
			maxProbability += e.probability;

		//Generate random number up to max probability
		float num = Random.Range(0, maxProbability);

		//Get random tile using cumulative probability
		float runningProbability = 0;
		foreach (Probability e in possibleGameObjects)
		{
			if (num >= runningProbability)
				possibleGameObject = e;

			runningProbability += e.probability;
		}

		return possibleGameObject.prefab;
	}
}