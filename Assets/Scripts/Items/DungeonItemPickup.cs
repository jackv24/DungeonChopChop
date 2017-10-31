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

    public float pickupStartDelay = 0.1f;
    private float pickupStartTime;

    private Coroutine pickupRoutine = null;

    void OnEnable()
    {
        pickupStartTime = Time.time + pickupStartDelay;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player1" || col.tag == "Player2")
        {
            float time = pickupStartTime - Time.time;

            time = Mathf.Max(time, 0);

            if (pickupRoutine == null)
                pickupRoutine = StartCoroutine(PickupDelayed(time));
        }
    }

    IEnumerator PickupDelayed(float time)
    {
        yield return new WaitForSeconds(time);

        if (itemType == DungeonItemType.Goggles)
        {
            ItemsManager.Instance.hasGoggles = true;
        }
        else if (itemType == DungeonItemType.Boots)
        {
            ItemsManager.Instance.hasBoots = true;
        }
        else if (itemType == DungeonItemType.Armor)
        {
            ItemsManager.Instance.hasArmourPiece = true;
        }
        else if (itemType == DungeonItemType.Gauntlet)
        {
            ItemsManager.Instance.hasGauntles = true;
        }
        gameObject.SetActive(false);
    }
}
