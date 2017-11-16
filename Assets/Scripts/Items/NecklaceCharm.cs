using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecklaceCharm : PickupableItems {

    [Space()]
    public GameObject pickupUIPrefab;

	// Use this for initialization
	void OnEnable () 
    {
        DidEnable();

        canPickup = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (canPickup)
        {
            //checks if the player collides with the item
            if (col.tag == "Player1" || col.tag == "Player2")
            {
                canPickup = false;

                CharmImage[] charmImages = FindObjectsOfType<CharmImage>();

                foreach (PlayerInformation player in GameManager.Instance.players)
                {
                    player.charmAmount++;

                    foreach (CharmImage charm in charmImages)
                    {
                        if (charm.id == player.playerIndex)
                            charm.UpdateCharms(player);
                    }
                }

                DoPickUpParticle();

                Destroy(gameObject);
            }
        }
    }

    void DoPickUpParticle()
    {
        //do particles
        SpawnEffects.EffectOnDeath(particles, transform.position);

        SoundManager.PlaySound(pickUpSound, transform.position);
    }
}
