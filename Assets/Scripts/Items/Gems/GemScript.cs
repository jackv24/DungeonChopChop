using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : PickupableItems {

	public int coinAmount;

    private int counter = 0;

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

    void FixedUpdate()
    {
        counter++;
        if (counter > pickupDelay * 60)
        {
            canPickup = true;
            Physics.IgnoreLayerCollision(15, 14, false);
        }
        //check if the velocity is greaters then the max, if so reset it to max
        if (rb.velocity.magnitude > maxVel)
        {
            rb.velocity.Normalize();
            rb.velocity = rb.velocity * maxVel;
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
