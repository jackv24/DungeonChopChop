using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
	[System.Serializable]
	public class ProbabilityGameObject
	{
		public GameObject prefab;
		[Tooltip("If probability = 0, it will be considered 1.")]
		public float probability;

		public ProbabilityGameObject()
		{
			probability = 1.0f;
		}
	}

	[System.Serializable]
	public class ProbabilityItem
	{
		public BaseItem item;
		[Tooltip("If probability = 0, it will be considered 1.")]
		public float probability;

		public ProbabilityItem()
		{
			probability = 1.0f;
		}
	}

	public static GameObject GetRandomGameObjectByProbability(ProbabilityGameObject[] array)
	{
		List<ProbabilityGameObject> possibleGameObjects = new List<ProbabilityGameObject>(array);

		ProbabilityGameObject possibleGameObject = null;

		///Get random enemy with probability
		//Sort possible enemy list by probability
		possibleGameObjects.Sort((x, y) => x.probability.CompareTo(y.probability));

		//Get sum of all probabilities
		float maxProbability = 0;
		foreach (ProbabilityGameObject e in possibleGameObjects)
			maxProbability += e.probability != 0 ? e.probability : 1;

		//Generate random number up to max probability
		float num = Random.Range(0, maxProbability);

		//Get random tile using cumulative probability
		float runningProbability = 0;
		foreach (ProbabilityGameObject e in possibleGameObjects)
		{
			if (num >= runningProbability)
				possibleGameObject = e;

			runningProbability += e.probability;
		}

		return possibleGameObject.prefab;
	}

	public static BaseItem GetRandomItemByProbability(ProbabilityItem[] array)
	{
		List<ProbabilityItem> possibleGameObjects = new List<ProbabilityItem>(array);

		ProbabilityItem possibleGameObject = null;

		///Get random enemy with probability
		//Sort possible enemy list by probability
		possibleGameObjects.Sort((x, y) => x.probability.CompareTo(y.probability));

		//Get sum of all probabilities
		float maxProbability = 0;
		foreach (ProbabilityItem e in possibleGameObjects)
			maxProbability += e.probability != 0 ? e.probability : 1;

		//Generate random number up to max probability
		float num = Random.Range(0, maxProbability);

		//Get random tile using cumulative probability
		float runningProbability = 0;
		foreach (ProbabilityItem e in possibleGameObjects)
		{
			if (num >= runningProbability)
				possibleGameObject = e;

			runningProbability += e.probability;
		}

		return possibleGameObject.item;
	}
}