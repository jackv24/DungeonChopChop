using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum DungeonItemType
{
    Goggles,
    Boots,
    Sword,
    Armor
}

public class DungeonItemPickup : MonoBehaviour
{
    public DungeonItemType itemType;

    // Use this for initialization
    void Start()
    {
		
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player1" || col.tag == "Player2")
        {
            if (itemType == DungeonItemType.Goggles)
            {
                ItemsManager.Instance.hasGoggles = true;
            }
            else if (itemType == DungeonItemType.Boots)
            {
                ItemsManager.Instance.hasBoots = true;
            }
            gameObject.SetActive(false);
        }
    }
}
