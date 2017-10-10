using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum OrbType {Health, Cure}

public class Orb : PickupableItems {

    public OrbType type;

    [HideInInspector]
    public float healthAmount;
    [HideInInspector]
    public int cureAmount;

    public GameObject particleOnCollect;

    private int counter = 0;

    void OnEnable()
    {
        counter = 0;
        canPickup = false;
    }
	
	// Update is called once per frame
	void FixedUpdate () 
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
            if (col.tag == "Player1" || col.tag == "Player2")
            {
                PlayerInformation playerInfo = col.gameObject.GetComponent<PlayerInformation>();
                //check what type of orb it is
                if (type == OrbType.Health)
                    col.gameObject.GetComponent<Health>().health += healthAmount;
                else
                {
                    if (playerInfo.currentCureOrbs < playerInfo.maxCureOrbs)
                    {
                        playerInfo.currentCureOrbs += cureAmount;
                        playerInfo.CureOrbChanged();
                    }
                }

                //do particles
                GameObject particle = ObjectPooler.GetPooledObject(particleOnCollect);
                particle.GetComponent<ParticleFollowHost>().host = col.transform;
                particle.transform.position = transform.position;
                gameObject.SetActive(false);
            }
        }
    }
}
