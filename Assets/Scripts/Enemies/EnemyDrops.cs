using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Drops
{
	public GameObject Drop;
	[Tooltip("eg 0 (inclusive)")]
	public int minPercentage;
	[Tooltip("eg 30 (inclusive)")]
	public int maxPercentage;
};

[Serializable]
public class PercentageOfDropAmount
{
	public int dropAmount;
	[Tooltip("eg 0 (inclusive)")]
	public int minPercentage;
	[Tooltip("eg 30 (inclusive)")]
	public int maxPercentage;
};

public class EnemyDrops : MonoBehaviour {

	public Drops[] drops;
	public PercentageOfDropAmount[] numberOfDrops;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DoDrop()
	{
		int dropAmount = 0;
		//gets a number between 0 and 100 which will act as a percentage
		int randomPercent = UnityEngine.Random.Range (0, 101);
		foreach (PercentageOfDropAmount drop in numberOfDrops) {
			//loop through each drop and find which drops min and max percentage have the percentage inbetween
			if (randomPercent >= drop.minPercentage && randomPercent <= drop.maxPercentage) {
				//this then finds how many items to drops
				dropAmount = drop.dropAmount;
			}
		}
		DropItem (dropAmount);
	}

	void DropItem(int dropAmount)
	{
		if (dropAmount != 0) 
		{
			//loop through the amount of items to drop
			for (int i = 0; i <= dropAmount; i++) 
			{
				//gets a number between 0 and 100 which will act as a percentage
				int randomPercent = UnityEngine.Random.Range (0, 101);
				foreach (Drops drop in drops)
				{
					//loop through each drop and find which drops min and max percentage have the percentage inbetween
					if (randomPercent >= drop.minPercentage && randomPercent <= drop.maxPercentage)
					{
						//creates the item and sets the position to the enemies position
						GameObject item = ObjectPooler.GetPooledObject (drop.Drop);
						item.transform.position = transform.position;
						break;
					}
				}
			}
		}
	}
}
