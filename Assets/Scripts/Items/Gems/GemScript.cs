﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : PickupableItems {

	public int coinAmount;

    private int counter = 0;

    void OnEnable()
    {
        counter = 0;
        canPickup = false;
        DidEnable();
    }

    void FixedUpdate()
    {
        counter++;
        if (counter > pickupDelay * 60)
        {
            canPickup = true;
        }
    }

	void OnTriggerEnter(Collider col)
	{
        if (canPickup)
        {
            //checks if the player collides with the item
            if (col.tag == "Player1" || col.tag == "Player2")
            {
                ItemsManager.Instance.Coins += coinAmount * (int)col.GetComponent<PlayerInformation>().GetCharmFloat("coinMultiplier");
                DoPickUpParticle();
                gameObject.SetActive(false);
            }
        }
	}

    void DoPickUpParticle()
    {
        //do particles
        spawnEffects.EffectOnDeath(particles, transform.position);
    }
}
