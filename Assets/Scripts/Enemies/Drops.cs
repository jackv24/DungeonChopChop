using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DropKinds
{
    public GameObject Drop;
    [Range(0, 100)]
    public int dropChance;
};

public class Drops : MonoBehaviour
{

    public DropKinds[] drops;
    [Tooltip("0 to this value")]
    public int maxAmountOfDrops;

    Collider col;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<Collider>();
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }

    public void DoDrop()
    {
        int randomDropAmount = UnityEngine.Random.Range(0, maxAmountOfDrops);
        if (maxAmountOfDrops > 0)
        {
            if (randomDropAmount > 0)
            {
                float percentage = 100 / randomDropAmount;
                float counter = 100;
                while (counter > 1)
                {
                    DropItem();
                    counter -= percentage;
                }
            }
        }
    }

    void DropItem()
    {
        //gets a number between 0 and 100 which will act as a percentage
        int randomPercent = 0;
        for (int j = 0; j < drops.Length; j++)
        {
            randomPercent = UnityEngine.Random.Range(0, 101);
            Debug.Log(randomPercent);
            //loop through each drop and find which drops min and max percentage have the percentage inbetween
            if (j != 0)
            {
                if (randomPercent <= drops[j].dropChance && randomPercent > drops[j - 1].dropChance)
                {
                    Drop(j);
                    break;
                }
            }
            else
            {
                Drop(j);
            }
        }
    }

    void Drop(int number)
    {
        //creates the item and sets the position to the enemies position
        GameObject item = ObjectPooler.GetPooledObject(drops[number].Drop);
        float x = UnityEngine.Random.Range(col.bounds.min.x, col.bounds.max.x);
        float y = UnityEngine.Random.Range(col.bounds.min.y, col.bounds.max.y);
        float z = UnityEngine.Random.Range(col.bounds.min.z, col.bounds.max.z);
        item.transform.position = new Vector3(x, y, z);
    }
}
