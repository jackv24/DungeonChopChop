using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour {

    PlayerAttack playerAttack;

	// Use this for initialization
	void Start () {
        playerAttack = GetComponentInParent<PlayerAttack>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySlash()
    {
        playerAttack.DisplaySlash();
    }
}
