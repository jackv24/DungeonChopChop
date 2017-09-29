﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPickup : MonoBehaviour
{

    public bool canPickUp;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Pickup(PlayerInformation playerInfo)
	{
		PlayerAttack playerAttack = playerInfo.GetComponent<PlayerAttack>();

		playerAttack.AddSword(GetComponent<SwordStats>());
	}
}
