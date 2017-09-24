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

    public void DoDrop()
    {
        //checks if the number of drops is 0
        int randomDropAmount = 0;
        if (maxAmountOfDrops == 1)
            randomDropAmount = UnityEngine.Random.Range(0, maxAmountOfDrops + 1);
        else
            randomDropAmount = UnityEngine.Random.Range(0, maxAmountOfDrops);

        if (maxAmountOfDrops > 0)
        {
            if (randomDropAmount > 0)
            {
                //gets the percentage, eg if number = 3, 100 / 3 = 33.333
                float percentage = 100 / randomDropAmount;
                float counter = 100;
                while (counter > 1)
                {
                    //do drop
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
