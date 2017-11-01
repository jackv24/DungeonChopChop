using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeMan : MonoBehaviour {

    public string welcomeMessage;
    public string messageAfterCollectingLoot;
    public bool dropCoins = false;
    public int cashAmount;

    private DialogueSpeaker dialogueSpeaker;
    private bool looted = false;
    private Drops drop;

	// Use this for initialization
	void Start () 
    {
        dialogueSpeaker = GetComponent<DialogueSpeaker>();
        drop = GetComponent<Drops>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!looted)
        {
            if (ItemsManager.Instance.hasBoots ||
                ItemsManager.Instance.hasGoggles ||
                ItemsManager.Instance.hasArmourPiece ||
                ItemsManager.Instance.hasGauntles)
            {
                dialogueSpeaker.lines[0] = messageAfterCollectingLoot;
            }
            else
            {
                dialogueSpeaker.lines[0] = welcomeMessage;
            }

            if (dialogueSpeaker.CurrentPlayer)
            {
                if (dialogueSpeaker.CurrentPlayer.playerMove.input.Purchase)
                {
                    if (dropCoins)
                        drop.DoDrop();
                    else
                        ItemsManager.Instance.Coins += cashAmount;
                    looted = true;
                }
            }
        }
        else
        {
            dialogueSpeaker.lines[0] = messageAfterCollectingLoot;
        }

        dialogueSpeaker.UpdateLines();
	}
}
