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
    private DialogueSpeaker dialogueSpeaker;

	// Use this for initialization
	void Start () {
        dialogueSpeaker = GetComponent<DialogueSpeaker>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (dialogueSpeaker.CurrentPlayer)
        {
            if (dialogueSpeaker.CurrentPlayer.playerMove.input.Purchase)
            {
                foreach (PlayerInformation player in GameManager.Instance.players)
                {
                    if (move == Moves.Dash)
                        player.playerAttack.canDash = true;
                    else if (move == Moves.DashAttack)
                        player.playerAttack.canDashAttack = true;
                    else if (move == Moves.Rapid)
                        player.playerAttack.canTripleAttack = true;
                    else if (move == Moves.Spin)
                        player.playerAttack.canSpinAttack = true;
                }
            }
        }
	}
}
