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
        //push the item out a bit
        Vector3 direction = new Vector3(Random.insideUnitSphere.x, 1, Random.insideUnitSphere.z);
        GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);

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

                PickUpOrb(playerInfo);

                DoPickUpParticle(col.gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    public void PickUpOrb(PlayerInformation playerInfo)
    {
        //check what type of orb it is
        if (type == OrbType.Health)
        {
            playerInfo.GetComponent<Health>().health += healthAmount;
            playerInfo.GetComponent<Health>().HealthChanged();
        }
        else
        {
            if (playerInfo.currentCureAmount < playerInfo.maxCureAmount)
            {
                playerInfo.currentCureAmount += cureAmount;
                playerInfo.CureOrbChanged();
            }
        }
    }

    void DoPickUpParticle(GameObject obj)
    {
        //do particles
        GameObject particle = ObjectPooler.GetPooledObject(spawnEffects.GetEffectOnDeath(particles));
        particle.GetComponent<ParticleFollowHost>().host = obj.transform;
        particle.transform.position = obj.transform.position;

        SoundManager.PlaySound(pickUpSound, transform.position);
    }
}
