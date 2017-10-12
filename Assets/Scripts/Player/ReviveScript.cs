using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveScript : MonoBehaviour {

    private DialogueSpeaker dialogueSpeaker;
    private PlayerInformation thisPlayer;
    private bool spawnedDialogue = false;
    private GameObject obj;

	// Use this for initialization
	void Start () {
        thisPlayer = GetComponent<PlayerInformation>();
        thisPlayer.playerMove.playerHealth.OnHealthChange += DisplayDialogue;
	}
	
	// Update is called once per frame
	void Update () {
        if (spawnedDialogue)
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                if (player != thisPlayer)
                {
                    if (player.playerMove.input.Purchase.WasPressed)
                    {
                        player.Revive(thisPlayer, player);
                        Destroy(obj);
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
                if (GameManager.Instance.players.Count > 0)
                {
                    obj = Instantiate(Resources.Load<GameObject>("ReviveObject"), transform.position, Quaternion.Euler(0, 0, 0));
                    spawnedDialogue = true;      
                }
            }
        }
    }
}
