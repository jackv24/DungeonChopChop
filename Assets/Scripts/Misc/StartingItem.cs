using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingItem : MonoBehaviour {

    public static StartingItem Instance;
    [Tooltip("Wait time to add items to play")]
    public float waitTime = 2;

    public Prize prize;

    [HideInInspector]
    public bool itemsAdded = false;

    public void AddStartingItems()
    {
        if (!itemsAdded)
            StartCoroutine(AddItems());
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        Instance = this;
    }

    IEnumerator AddItems()
    {
        yield return new WaitForSeconds(waitTime);

        //find which items are occupied, then add them to the player(s)
        if (prize.sword)
            AddSword();
        if (prize.shield)
            AddShield();
        if (prize.item)
            AddInventoryItem();
        if (prize.charm)
            AddCharm();
        if (prize.coins > 0 || prize.dungeonKeys > 0 || prize.keys > 0)
            AddVars();

        itemsAdded = true;
    }

    void AddSword()
    {
        foreach (PlayerInformation player in GameManager.Instance.players)
        {
            GameObject s = (GameObject)Instantiate(prize.sword.gameObject, transform.position, Quaternion.Euler(0, 0, 0));
            player.playerAttack.AddSword(s.GetComponent<SwordStats>());
        }
    }

    void AddShield()
    {
        foreach (PlayerInformation player in GameManager.Instance.players)
        {
            GameObject s = (GameObject)Instantiate(prize.shield.gameObject, transform.position, Quaternion.Euler(0, 0, 0));
            player.playerAttack.AddShield(s.GetComponent<ShieldStats>());
        }
    }

    void AddInventoryItem()
    {
        foreach (PlayerInformation player in GameManager.Instance.players)
        {
            if (prize.item.armourType == ArmourType.Boots || prize.item.armourType == ArmourType.ChestPiece)
                player.PickupItem(prize.item);
        }
    }

    void AddCharm()
    {
        foreach (PlayerInformation player in GameManager.Instance.players)
        {
            GameObject obj = ObjectPooler.GetPooledObject(LevelVars.Instance.droppedCharmPrefab);

            obj.transform.position = player.transform.position;

            CharmPickup pickup = obj.GetComponentInChildren<CharmPickup>();

            pickup.representingCharm = prize.charm;

            if (pickup)
            {
                pickup.Pickup(player);
            }
        }
    }

    void AddVars()
    {
        //add coins
        ItemsManager.Instance.Coins += prize.coins;
        ItemsManager.Instance.CoinChange();

        //add keys
        ItemsManager.Instance.Keys += prize.keys;
        ItemsManager.Instance.KeyChange();

        //add dungeon keys
        ItemsManager.Instance.DungeonKeys += prize.dungeonKeys;
        ItemsManager.Instance.DungeonKeyChange();
    }
}
