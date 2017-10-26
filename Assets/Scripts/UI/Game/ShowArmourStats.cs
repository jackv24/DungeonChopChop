using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowArmourStats : MonoBehaviour {

    private DialogueSpeaker dialogueSpeaker;
    private InventoryItem thisArmourStats;
    private ArmourPickup armourPickup;

    private float multiplier;
    private string var1string = "";

    private PlayerMove playerMove;

    // Use this for initialization
    void Start () 
    {
        armourPickup = GetComponent<ArmourPickup>();

        thisArmourStats = (InventoryItem)GetComponent<ItemPickup>().representingItem;
            
        dialogueSpeaker = GetComponent<DialogueSpeaker>();

        dialogueSpeaker.OnGetPlayer += SetDialgeText;
    }

    void Update()
    {
        if (dialogueSpeaker.CurrentPlayer != null)
        {
            if (dialogueSpeaker.CurrentPlayer.playerMove.input.Purchase.WasPressed)
            {
                armourPickup.Pickup(dialogueSpeaker.CurrentPlayer);
                dialogueSpeaker.Close();
                enabled = false;
                dialogueSpeaker.enabled = false;
            }
        }
    }
    bool HasTypeInInventory(PlayerInformation player, ArmourType type)
    {
        foreach (InventoryItem item in player.currentItems)
        {
            if (item.armourType == type)
            {
                return true;
            }
        }

        return false;
    }

    void PositiveOrNegativeString(ref string str, ref Color col, float currentVal, float possibleVal)
    {
        float difference = Mathf.Abs(possibleVal - currentVal);

        difference = (float)System.Math.Round(difference, 2);

        if (currentVal == possibleVal)
        {
            col = Color.gray;
            str = "" + 0;
        }
        else if (possibleVal < currentVal)
        {
            col = Color.red;
            str = "-" + difference;
        }
        else if (possibleVal > currentVal)
        {
            col = Color.green;
            str = "+" + difference;
        }
    }

    void SetDialgeText(PlayerInformation playerInfo, bool b)
    {
        Color newCol = Color.green;

        string descriptionText = "";

        if (playerInfo.currentItems.Count > 0)
        {
            foreach (InventoryItem i in playerInfo.currentItems)
            {
                if (HasTypeInInventory(playerInfo, thisArmourStats.armourType))
                {
                    for (int j = 0; j < thisArmourStats.items.Length; j++)
                    {
                        PositiveOrNegativeString(ref var1string, ref newCol, i.items[j].floatValue, thisArmourStats.items[j].floatValue);

                        descriptionText += string.Format(
                            "{2}: <color=#{0}>{1}</color>\n",
                            ColorUtility.ToHtmlStringRGB(newCol),
                            var1string,
                            i.items[j].itemKey
                        );
                    }
                }
                else
                {
                    for (int j = 0; j < thisArmourStats.items.Length; j++)
                    {
                        PositiveOrNegativeString(ref var1string, ref newCol, 0, thisArmourStats.items[j].floatValue);

                        descriptionText += string.Format(
                            "{2}: <color=#{0}>{1}</color>\n",
                            ColorUtility.ToHtmlStringRGB(newCol),
                            var1string,
                            thisArmourStats.items[j].itemKey
                        );
                    }
                }
            }
        }
        else
        {
            for (int j = 0; j < thisArmourStats.items.Length; j++)
            {
                PositiveOrNegativeString(ref var1string, ref newCol, 0, thisArmourStats.items[j].floatValue);

                descriptionText += string.Format(
                    "{2}: <color=#{0}>{1}</color>\n",
                    ColorUtility.ToHtmlStringRGB(newCol),
                    var1string,
                    thisArmourStats.items[j].itemKey
                );
            }
        }
    }
}
