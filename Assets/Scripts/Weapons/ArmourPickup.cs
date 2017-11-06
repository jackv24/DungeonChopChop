using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourPickup : MonoBehaviour
{
    public bool canPickUp;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    public void Pickup(PlayerInformation playerInfo)
    {
        ItemPickup itemPickup = GetComponent<ItemPickup>();

        if (itemPickup)
        {
            playerInfo.PickupItem((InventoryItem)itemPickup.representingItem);
        }
    }
}
