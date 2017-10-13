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

    private int counter = 0;

    private Collider c;

    void OnEnable()
    {
        counter = 0;
        canPickup = false;
        DidEnable();
        Physics.IgnoreLayerCollision(15, 14, true);
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(.1f);
        DoSpawnParticle(transform.position);
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        counter++;
        if (counter > pickupDelay * 60)
        {
            Physics.IgnoreLayerCollision(15, 14, false);
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
                {
                    col.gameObject.GetComponent<Health>().health += healthAmount;
                    col.gameObject.GetComponent<Health>().HealthChanged();
                }
                else
                {
                    if (playerInfo.currentCureOrbs < playerInfo.maxCureOrbs)
                    {
                        playerInfo.currentCureOrbs += cureAmount;
                        playerInfo.CureOrbChanged();
                    }
                }
                DoPickUpParticle(col.gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    void DoPickUpParticle(GameObject obj)
    {
        //do particles
        GameObject particle = ObjectPooler.GetPooledObject(spawnEffects.GetEffectOnDeath(particles));
        particle.GetComponent<ParticleFollowHost>().host = obj.transform;
        particle.transform.position = obj.transform.position;
    }
}
