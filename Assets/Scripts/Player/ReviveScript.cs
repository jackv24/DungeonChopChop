using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveScript : MonoBehaviour
{
    private DialogueSpeaker dialogueSpeaker;
    private PlayerInformation thisPlayer;
    private bool spawnedDialogue = false;
    private GameObject obj;

	void Start ()
    {
        thisPlayer = GetComponent<PlayerInformation>();

        if(thisPlayer)
            thisPlayer.playerMove.playerHealth.OnHealthChange += DisplayDialogue;
	}

	void Update ()
    {
        if (spawnedDialogue)
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                if (player.playerIndex != thisPlayer.playerIndex)
                {
                    if (player.playerMove.input.Purchase.WasPressed)
                    {
                        player.Revive(thisPlayer, player);

                        Destroy(obj);
                        spawnedDialogue = false;

                        break;
                    }
                }
            }
        }
	}

    void DisplayDialogue()
    {
        if (!spawnedDialogue)
        {
            if (thisPlayer.playerMove.playerHealth.health <= 0)
            {
                if (GameManager.Instance.players.Count > 1)
                {
                    obj = Instantiate(Resources.Load<GameObject>("ReviveObject"), transform.position, Quaternion.Euler(0, 0, 0));

                    dialogueSpeaker = obj.GetComponent<DialogueSpeaker>();

                    spawnedDialogue = true;      
                }
            }
        }
    }
}
