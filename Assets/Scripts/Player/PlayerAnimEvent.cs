using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour {

    PlayerAttack playerAttack;
    PlayerInformation playerInfo;

	// Use this for initialization
	void Start () {
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerInfo = GetComponentInParent<PlayerInformation>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySlash()
    {
        playerAttack.DisplaySlash();
    }

    public void EnableSword()
    {
        playerAttack.EnableSword();
    }

    public void DisableSword()
    {
        playerAttack.DisableSword();
    }
}
