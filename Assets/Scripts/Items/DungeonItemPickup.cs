using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum DungeonItemType
{
    Goggles,
    Boots,
    Gauntlet,
    Armor
}

public class DungeonItemPickup : MonoBehaviour
{
    public DungeonItemType itemType;
	public GameObject itemPurchasePopup;
	public InventoryItem item;

    public float pickupStartDelay = 0.1f;
    private float pickupStartTime;

    private Coroutine pickupRoutine = null;

    void OnEnable()
    {
        pickupStartTime = Time.time + pickupStartDelay;
    }

    void OnTriggerEnter(Collider col)
    {
        //picks up the dungeon item
        if (col.tag == "Player1" || col.tag == "Player2")
        {
            float time = pickupStartTime - Time.time;

            time = Mathf.Max(time, 0);

            if (pickupRoutine == null)
				pickupRoutine = StartCoroutine(PickupDelayed(time, col.GetComponent<PlayerInformation>()));
        }
    }

	IEnumerator PickupDelayed(float time, PlayerInformation playerInfo)
    {
        yield return new WaitForSeconds(time);

        Chest chest = GetComponentInParent<Chest>();

        SpawnEffects.EffectOnHit(chest.poofParticle, transform.position);

        chest.gameObject.SetActive(false);

        //choses what dungeon item it is
        if (itemType == DungeonItemType.Goggles)
            ItemsManager.Instance.hasGoggles = true;
        else if (itemType == DungeonItemType.Boots)
            ItemsManager.Instance.hasBoots = true;
        else if (itemType == DungeonItemType.Armor)
            ItemsManager.Instance.hasArmourPiece = true;
        else if (itemType == DungeonItemType.Gauntlet)
            ItemsManager.Instance.hasGauntles = true;

        //does the pop up icon
		if(itemPurchasePopup)
		{
			GameObject obj = ObjectPooler.GetPooledObject(itemPurchasePopup);

			ShopItemPopup popup = obj.GetComponent<ShopItemPopup>();
			if (popup)
				popup.Init(item, playerInfo.transform);
		}

        ItemsManager.Instance.dungeonItems.Add(itemType.ToString(), false);

        gameObject.SetActive(false);
    }
}
