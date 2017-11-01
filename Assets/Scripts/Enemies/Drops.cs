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
    public Helper.ProbabilityGameObject[] drops;
    public int minAmountOfDrops;
    public int maxAmountOfDrops;

    [Space()]
    public bool isWizzer = false;

    private Collider col;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        if (isWizzer)
            DoDrop();
    }

    int addPercentage(int number)
    {
        if (ItemsManager.Instance.itemDropMultiplier == 0)
            return 0;
        else if (ItemsManager.Instance.itemDropMultiplier == 1)
            return number;
        else
        {
            return (int)(number * ItemsManager.Instance.itemDropMultiplier) / 100;
        }
    }

    public void DoDrop()
    {
        //checks if the number of drops is 0
        int randomDropAmount = 0;
        if (maxAmountOfDrops == 1)
            randomDropAmount = UnityEngine.Random.Range(addPercentage(minAmountOfDrops), addPercentage(maxAmountOfDrops) + 1);
        else
            randomDropAmount = UnityEngine.Random.Range(addPercentage(minAmountOfDrops), addPercentage(maxAmountOfDrops) + 1);

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
        GameObject obj = Helper.GetRandomGameObjectByProbability(drops);
        Drop(obj);
    }

    void Drop(GameObject obj)
    {
        //creates the item and sets the position to the enemies position
        GameObject item = ObjectPooler.GetPooledObject(obj);

        item.transform.position = transform.position;
    }
}
