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

		[Tooltip("The maximum amount of this gameobject to be chosen. If 0 it is considered infinite.")]
        public int maxAmount = 0;
        private int chosenAmount = 0;

		public bool CanChoose
		{
			get
			{
				//This boject can be chosen if max is 0, or chosen is less than max
				if (maxAmount <= 0)
                    return true;
				else if (chosenAmount < maxAmount)
                    return true;

				//Otherwise it can not be chosen
                return false;
            }
		}

        public ProbabilityGameObject()
		{
			probability = 1.0f;
            maxAmount = 0;
        }

		public void IncrementChosenAmount()
		{
            chosenAmount++;
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
		List<ProbabilityGameObject> possibleGameObjects = new List<ProbabilityGameObject>();

		//Only try and choose gameobjects that can be chosen
		foreach(ProbabilityGameObject obj in array)
		{
            if (obj.CanChoose)
            {
				//Make sure this object can only be chosen the specified number of times
                possibleGameObjects.Add(obj);
                obj.IncrementChosenAmount();
            }
        }

        if (possibleGameObjects.Count > 0)
        {
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
		else
		{
            Debug.LogError("There were no possible gameobjects to choose from!");

            return null;
        }
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

    public static float LinearToDecibel(float linear)
    {
        float dB;

        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }
}