using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : PickupableItems {

    public float healthAmount;
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
                col.gameObject.GetComponent<Health>().health += healthAmount;
                GameObject particle = ObjectPooler.GetPooledObject(particleOnCollect);
                particle.GetComponent<ParticleFollowHost>().host = col.transform;
                particle.transform.position = transform.position;
                gameObject.SetActive(false);
            }
        }
    }
}
