using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class Prize
{
    public bool randomizeAll = true;
    [Space()]
    public SwordStats sword;
    public bool randomSword = false;

    [Space()]
    public ShieldStats shield;
    public bool randomShield = false;

    [Space()]
    public InventoryItem item;
    public bool randomItem = false;

    [Space()]
    public Charm charm;
    public bool randomCharm = false;

    [Space()]
    public int coins;
    public int keys;
    public int dungeonKeys;
}

public class WheelOfFortune : MonoBehaviour
{
    public List<Prize> prizes;
    public List<AnimationCurve> animationCurves;
    public Text playAgainText;

    [Header("All Possible Items")]
    public SwordStats[] allSwords;
    public ShieldStats[] allShields;
    public InventoryItem[] allItems;
    public Charm[] allCharms;

    private StartingItem startingItem;

    private bool spinning;    
    private float anglePerItem;    
    private int randomTime;
    private int itemNumber;

    void Start()
    {
        spinning = false;
        anglePerItem = 360/prizes.Count;        
    }

    public void SpinWheel()
    {
        randomTime = Random.Range (1, 4);
        itemNumber = Random.Range (0, prizes.Count);
        float maxAngle = 360 * randomTime + (itemNumber * anglePerItem);

        StartCoroutine (SpinTheWheel (5 * randomTime, maxAngle));
    }

    IEnumerator SpinTheWheel (float time, float maxAngle)
    {

        startingItem = GameObject.FindObjectOfType<StartingItem>();

        startingItem.prize = prizes[itemNumber];
        startingItem.itemsAdded = false;

        spinning = true;

        float timer = 0.0f;        
        float startAngle = transform.eulerAngles.z;        
        maxAngle = maxAngle - startAngle;

        int animationCurveNumber = Random.Range (0, animationCurves.Count);

        while (timer < time) {
            //to calculate rotation
            float angle = maxAngle * animationCurves [animationCurveNumber].Evaluate (timer / time) ;
            transform.eulerAngles = new Vector3 (0.0f, 0.0f, angle + startAngle);
            timer += Time.deltaTime;
            yield return 0;
        }

        transform.eulerAngles = new Vector3 (0.0f, 0.0f, maxAngle + startAngle);
        spinning = false;

        //randomize the prize
        if (prizes[itemNumber].randomizeAll)
            Randomize();
        else
            OtherRandomItems(); 

        Debug.Log(itemNumber + " Won");

        DoText();
    }    

    void DoText()
    {
        if (playAgainText)
        {
            //get the correct prize and set the text
            if (startingItem.prize.sword)
                playAgainText.text = "TAKE THIS ITEM TO THE AFTERLIFE: <color=grey>'" + startingItem.prize.sword.swordName + "'</color>";
            else if (startingItem.prize.shield)
                playAgainText.text = "TAKE THIS ITEM TO THE AFTERLIFE: <color=grey>'" + startingItem.prize.shield.shieldName + "'</color>";
            else if (startingItem.prize.item)
                playAgainText.text = "TAKE THIS ITEM TO THE AFTERLIFE: <color=grey>'" + startingItem.prize.item.displayName + "'</color>";
            else if (startingItem.prize.charm)
                playAgainText.text = "TAKE THIS ITEM TO THE AFTERLIFE: <color=grey>'" + startingItem.prize.charm.displayName + "'</color> charm";
            else if (startingItem.prize.coins > 0)
                playAgainText.text = "TAKE THIS ITEM TO THE AFTERLIFE: <color=grey>'" + startingItem.prize.coins + "'</color> coins";
            else if (startingItem.prize.keys > 0)
                playAgainText.text = "TAKE THIS ITEM TO THE AFTERLIFE: <color=grey>'" + startingItem.prize.keys + "'</color> key";
            else if (startingItem.prize.dungeonKeys > 0)
                playAgainText.text = "TAKE THIS ITEM TO THE AFTERLIFE: <color=grey>'" + startingItem.prize.dungeonKeys + "'</color> Dungeon key";

            playAgainText.gameObject.SetActive(true);
        }
    }

    void OtherRandomItems()
    {
        if (prizes[itemNumber].randomSword)
            prizes[itemNumber].sword = RandomSword();
        else if (prizes[itemNumber].randomShield)
            prizes[itemNumber].shield = RandomShield();
        else if (prizes[itemNumber].randomItem)
            prizes[itemNumber].item = RandomItem();
        else if (prizes[itemNumber].randomCharm)
            prizes[itemNumber].charm = RandomCharm();
    }

    void Randomize()
    {
        int randomOption = Random.Range(0, 5);

        if (randomOption == 0)
            prizes[itemNumber].sword = RandomSword();
        else if (randomOption == 1)
            prizes[itemNumber].shield = RandomShield();
        else if (randomOption == 2)
            prizes[itemNumber].item = RandomItem();
        else if (randomOption == 3)
            prizes[itemNumber].charm = RandomCharm();
        else if (randomOption == 4)
            RandomVar(itemNumber);
    }

    SwordStats RandomSword()
    {
        int random = Random.Range(0, allSwords.Length);

        return allSwords[random];
    }

    ShieldStats RandomShield()
    {
        int random = Random.Range(0, allShields.Length);

        return allShields[random];
    }

    InventoryItem RandomItem()
    {
        int random = Random.Range(0, allItems.Length);

        return allItems[random];
    }

    Charm RandomCharm()
    {
        int random = Random.Range(0, allCharms.Length);

        return allCharms[random];
    }

    void RandomVar(int number)
    {
        prizes[number].coins = (int)Random.Range(prizes[number].coins / 2, prizes[number].coins * 2);
        prizes[number].keys = (int)Random.Range(prizes[number].keys / 2, prizes[number].keys * 2);
        prizes[number].dungeonKeys = (int)Random.Range(prizes[number].dungeonKeys / 2, prizes[number].dungeonKeys * 2);
    }
}