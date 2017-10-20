using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
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

	public static GameObject GetRandomGameObjectByProbability(ProbabilityGameObject[] array, System.Random random = null)
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
		float num;
		if (random != null)
			num = random.NextFloat(0, maxProbability);
		else
			num = Random.Range(0, maxProbability);

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

	public static BaseItem GetRandomItemByProbability(ProbabilityItem[] array, System.Random random = null)
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
		float num;
		if (random != null)
			num = random.NextFloat(0, maxProbability);
		else
			num = Random.Range(0, maxProbability);

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

	public static float NextFloat(this System.Random random, float min, float max)
	{
		double range = (double)max - (double)min;
		double sample = random.NextDouble();
		double scaled = (sample * range) + min;

		return (float)scaled;
	}

	public static void SetLayerWithChildren(this GameObject gameObject, int layer)
	{
        gameObject.layer = layer;

        for (int i = 0; i < gameObject.transform.childCount; i++)
		{
            gameObject.transform.GetChild(i).gameObject.SetLayerWithChildren(layer);
        }
    }

	public static void DestroyChildren(this GameObject gameObject)
	{
		for (int i = gameObject.transform.childCount - 1; i >= 0 ; i--)
		{
            GameObject.Destroy(gameObject.transform.GetChild(i).gameObject);
        }
	}
}