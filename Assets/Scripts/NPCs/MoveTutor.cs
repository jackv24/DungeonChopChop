using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Moves
{
    Dash,
    DashAttack,
    Rapid,
    Spin
}

public class MoveTutor : MonoBehaviour {

    public Moves move;
    public string afterSpeach;

    private DialogueSpeaker dialogueSpeaker;
    private string originalSpeach;
    private bool taught = false;

	// Use this for initialization
	void Start () {
        dialogueSpeaker = GetComponent<DialogueSpeaker>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (move == Moves.DashAttack)
        {
            if (!GameManager.Instance.players[0].playerAttack.canDash)
            {
                dialogueSpeaker.lines[0] = "Come back to me once you know how to dash";
            }
            else
                dialogueSpeaker.lines[0] = "Want me to teach you a move my boy?";

            dialogueSpeaker.UpdateLines();
        }


        if (dialogueSpeaker.CurrentPlayer)
        {
            if (dialogueSpeaker.CurrentPlayer.playerMove.input.Purchase)
            {
                foreach (PlayerInformation player in GameManager.Instance.players)
                {
                    if (move == Moves.Dash)
                        player.playerAttack.canDash = true;
                    else if (move == Moves.DashAttack)
                    {
                        if (player.playerAttack.canDash)
                            player.playerAttack.canDashAttack = true;
                    }
                    else if (move == Moves.Rapid)
                        player.playerAttack.canTripleAttack = true;
                    else if (move == Moves.Spin)
                        player.playerAttack.canSpinAttack = true;

                    taught = true;
                }
            }

            if (taught)
            {
                dialogueSpeaker.lines[0] = afterSpeach;

                dialogueSpeaker.UpdateLines();
            }
        }
	}
}
